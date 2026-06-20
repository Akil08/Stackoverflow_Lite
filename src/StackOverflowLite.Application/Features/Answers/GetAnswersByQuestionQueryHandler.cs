using MediatR;
using Microsoft.EntityFrameworkCore;
using StackOverflowLite.Application.Common.Interfaces;
using StackOverflowLite.Application.Common.Models;
using StackOverflowLite.Domain.Enums;

namespace StackOverflowLite.Application.Features.Answers;

public class GetAnswersByQuestionQueryHandler : IRequestHandler<GetAnswersByQuestionQuery, Result<List<AnswerDto>>>
{
    private readonly IApplicationDbContext _db;

    public GetAnswersByQuestionQueryHandler(IApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<Result<List<AnswerDto>>> Handle(GetAnswersByQuestionQuery request, CancellationToken cancellationToken)
    {
        var answers = await _db.Answers
            .Where(x => x.QuestionId == request.QuestionId)
            .Include(x => x.Author)
            .OrderByDescending(x => x.IsAccepted)
            .ThenBy(x => x.CreatedAt)
            .Select(x => new AnswerDto
            {
                Id = x.Id,
                Content = x.Content,
                AuthorName = x.Author.UserName ?? string.Empty,
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt,
                IsAccepted = x.IsAccepted,
                VoteCount = _db.Votes.Count(v => v.TargetType == VoteTarget.Answer && v.TargetId == x.Id && v.VoteType == VoteType.Upvote)
                            - _db.Votes.Count(v => v.TargetType == VoteTarget.Answer && v.TargetId == x.Id && v.VoteType == VoteType.Downvote)
            })
            .ToListAsync(cancellationToken);

        return Result<List<AnswerDto>>.Success(answers);
    }
}