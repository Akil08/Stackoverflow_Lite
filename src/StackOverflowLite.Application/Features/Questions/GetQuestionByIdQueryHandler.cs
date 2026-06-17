using MediatR;
using Microsoft.EntityFrameworkCore;
using StackOverflowLite.Application.Common.Interfaces;
using StackOverflowLite.Application.Common.Models;
using StackOverflowLite.Domain.Enums;

namespace StackOverflowLite.Application.Features.Questions;

public class GetQuestionByIdQueryHandler : IRequestHandler<GetQuestionByIdQuery, Result<QuestionDetailDto>>
{
    private readonly IApplicationDbContext _db;

    public GetQuestionByIdQueryHandler(IApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<Result<QuestionDetailDto>> Handle(GetQuestionByIdQuery request, CancellationToken cancellationToken)
    {
        var question = await _db.Questions
            .Include(q => q.QuestionTags).ThenInclude(qt => qt.Tag)
            .Include(q => q.Author)
            .FirstOrDefaultAsync(q => q.Id == request.QuestionId, cancellationToken);

        if (question == null) return Result<QuestionDetailDto>.Failure("Question not found");

        // increment view count
        question.ViewCount += 1;
        await _db.SaveChangesAsync(cancellationToken);

        var voteCount = _db.Votes.Where(v => v.TargetType == VoteTarget.Question && v.TargetId == question.Id && v.VoteType == VoteType.Upvote).Count()
                        - _db.Votes.Where(v => v.TargetType == VoteTarget.Question && v.TargetId == question.Id && v.VoteType == VoteType.Downvote).Count();

        var answerCount = _db.Answers.Count(a => a.QuestionId == question.Id);

        var dto = new QuestionDetailDto
        {
            Id = question.Id,
            Title = question.Title,
            Description = question.Description,
            AuthorName = question.Author.UserName ?? string.Empty,
            CreatedAt = question.CreatedAt,
            UpdatedAt = question.UpdatedAt,
            Tags = question.QuestionTags.Select(qt => qt.Tag.Name).ToList(),
            VoteCount = voteCount,
            AnswerCount = answerCount,
            AcceptedAnswerId = question.AcceptedAnswerId,
            ViewCount = question.ViewCount
        };

        return Result<QuestionDetailDto>.Success(dto);
    }
}