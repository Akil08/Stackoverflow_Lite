using MediatR;
using Microsoft.EntityFrameworkCore;
using StackOverflowLite.Application.Common.Interfaces;
using StackOverflowLite.Application.Common.Models;
using StackOverflowLite.Application.Common.Services;
using StackOverflowLite.Domain.Entities;
using StackOverflowLite.Domain.Enums;

namespace StackOverflowLite.Application.Features.Votes;

public class VoteCommandHandler : IRequestHandler<VoteCommand, Result<bool>>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUserService;
    private readonly ReputationService _reputationService;

    public VoteCommandHandler(
        IApplicationDbContext db,
        ICurrentUserService currentUserService,
        ReputationService reputationService)
    {
        _db = db;
        _currentUserService = currentUserService;
        _reputationService = reputationService;
    }

    public async Task<Result<bool>> Handle(VoteCommand request, CancellationToken cancellationToken)
    {
        if (!_currentUserService.IsAuthenticated)
        {
            return Result<bool>.Failure("Not authenticated");
        }

        if (!Guid.TryParse(_currentUserService.UserId, out var currentUserId))
        {
            return Result<bool>.Failure("Invalid user id");
        }

        Question? question = null;
        Answer? answer = null;
        ApplicationUser? author = null;

        switch (request.TargetType)
        {
            case VoteTarget.Question:
                question = await _db.Questions.Include(x => x.Author).FirstOrDefaultAsync(x => x.Id == request.TargetId, cancellationToken);
                author = question?.Author;
                break;
            case VoteTarget.Answer:
                answer = await _db.Answers.Include(x => x.Author).FirstOrDefaultAsync(x => x.Id == request.TargetId, cancellationToken);
                author = answer?.Author;
                break;
        }

        if (author is null)
        {
            return Result<bool>.Failure("Target not found");
        }

        if (author.Id == currentUserId)
        {
            return Result<bool>.Failure("You cannot vote on your own content");
        }

        var existingVote = await _db.Votes.FirstOrDefaultAsync(
            x => x.UserId == currentUserId && x.TargetId == request.TargetId && x.TargetType == request.TargetType,
            cancellationToken);

        if (existingVote is null)
        {
            _db.Votes.Add(new Vote
            {
                Id = Guid.NewGuid(),
                UserId = currentUserId,
                TargetId = request.TargetId,
                TargetType = request.TargetType,
                VoteType = request.VoteType,
                CreatedAt = DateTime.UtcNow
            });

            _reputationService.ApplyReputationChange(author, request.TargetType, request.VoteType, isReversal: false);
        }
        else if (existingVote.VoteType == request.VoteType)
        {
            _db.Votes.Remove(existingVote);
            _reputationService.ApplyReputationChange(author, request.TargetType, existingVote.VoteType, isReversal: true);
        }
        else
        {
            _reputationService.ApplyReputationChange(author, request.TargetType, existingVote.VoteType, isReversal: true);
            existingVote.VoteType = request.VoteType;
            _reputationService.ApplyReputationChange(author, request.TargetType, request.VoteType, isReversal: false);
        }

        await _db.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }
}