using MediatR;
using Microsoft.EntityFrameworkCore;
using StackOverflowLite.Application.Common.Interfaces;
using StackOverflowLite.Application.Common.Models;
using StackOverflowLite.Domain.Entities;

namespace StackOverflowLite.Application.Features.Answers;

public class AcceptAnswerCommandHandler : IRequestHandler<AcceptAnswerCommand, Result<bool>>
{
    private const int AcceptedAnswerReputation = 15;

    private readonly IApplicationDbContext _dbContext;
    private readonly ICurrentUserService _currentUserService;

    public AcceptAnswerCommandHandler(IApplicationDbContext dbContext, ICurrentUserService currentUserService)
    {
        _dbContext = dbContext;
        _currentUserService = currentUserService;
    }

    public async Task<Result<bool>> Handle(AcceptAnswerCommand request, CancellationToken cancellationToken)
    {
        if (!_currentUserService.IsAuthenticated)
        {
            return Result<bool>.Failure("Not authenticated");
        }

        if (!Guid.TryParse(_currentUserService.UserId, out var currentUserId))
        {
            return Result<bool>.Failure("Invalid user id");
        }

        var question = await _dbContext.Questions
            .FirstOrDefaultAsync(q => q.Id == request.QuestionId, cancellationToken);

        if (question is null)
        {
            return Result<bool>.Failure("Question not found");
        }

        if (question.AuthorId != currentUserId)
        {
            return Result<bool>.Failure("Forbidden");
        }

        if (request.AnswerId is null)
        {
            if (question.AcceptedAnswerId is null)
            {
                return Result<bool>.Success(true);
            }

            var currentAcceptedAnswer = await _dbContext.Answers
                .Include(answer => answer.Author)
                .FirstOrDefaultAsync(answer => answer.Id == question.AcceptedAnswerId.Value && answer.QuestionId == question.Id, cancellationToken);

            if (currentAcceptedAnswer is not null)
            {
                currentAcceptedAnswer.IsAccepted = false;
                currentAcceptedAnswer.Author.Reputation = Math.Max(0, currentAcceptedAnswer.Author.Reputation - AcceptedAnswerReputation);
            }

            question.AcceptedAnswerId = null;

            await _dbContext.SaveChangesAsync(cancellationToken);

            return Result<bool>.Success(true);
        }

        var answer = await _dbContext.Answers
            .Include(answer => answer.Author)
            .FirstOrDefaultAsync(answer => answer.Id == request.AnswerId.Value && answer.QuestionId == question.Id, cancellationToken);

        if (answer is null)
        {
            return Result<bool>.Failure("Answer not found");
        }

        if (answer.AuthorId == question.AuthorId)
        {
            return Result<bool>.Failure("You cannot accept your own answer");
        }

        if (question.AcceptedAnswerId.HasValue && question.AcceptedAnswerId.Value != answer.Id)
        {
            var previousAcceptedAnswer = await _dbContext.Answers
                .Include(previous => previous.Author)
                .FirstOrDefaultAsync(previous => previous.Id == question.AcceptedAnswerId.Value && previous.QuestionId == question.Id, cancellationToken);

            if (previousAcceptedAnswer is not null)
            {
                previousAcceptedAnswer.IsAccepted = false;
                previousAcceptedAnswer.Author.Reputation = Math.Max(0, previousAcceptedAnswer.Author.Reputation - AcceptedAnswerReputation);
            }
        }

        if (question.AcceptedAnswerId != answer.Id)
        {
            answer.IsAccepted = true;
            answer.Author.Reputation += AcceptedAnswerReputation;
        }
        else
        {
            answer.IsAccepted = true;
        }

        question.AcceptedAnswerId = answer.Id;

        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }
}