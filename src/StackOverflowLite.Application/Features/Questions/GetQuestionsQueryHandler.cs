using MediatR;
using Microsoft.EntityFrameworkCore;
using StackOverflowLite.Application.Common.Models;
using StackOverflowLite.Application.Common.Interfaces;
using StackOverflowLite.Domain.Enums;

namespace StackOverflowLite.Application.Features.Questions;

public class GetQuestionsQueryHandler : IRequestHandler<GetQuestionsQuery, Result<PagedResult<QuestionSummaryDto>>>
{
    private readonly IApplicationDbContext _db;
    private readonly ICacheService _cacheService;

    public GetQuestionsQueryHandler(IApplicationDbContext db, ICacheService cacheService)
    {
        _db = db;
        _cacheService = cacheService;
    }

    public async Task<Result<PagedResult<QuestionSummaryDto>>> Handle(GetQuestionsQuery request, CancellationToken cancellationToken)
    {
        var cacheKey = $"questions:list:{request.TagName ?? "all"}:{request.Page}:{request.PageSize}";
        var cached = await _cacheService.GetAsync<PagedResult<QuestionSummaryDto>>(cacheKey, cancellationToken);
        if (cached is not null)
        {
            return Result<PagedResult<QuestionSummaryDto>>.Success(cached);
        }

        var query = _db.Questions.AsQueryable();

        if (!string.IsNullOrEmpty(request.TagName))
        {
            query = query.Where(q => q.QuestionTags.Any(qt => qt.Tag.Name == request.TagName));
        }

        var total = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(q => q.CreatedAt)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(q => new QuestionSummaryDto
            {
                Id = q.Id,
                Title = q.Title,
                AuthorName = q.Author.UserName ?? string.Empty,
                CreatedAt = q.CreatedAt,
                Tags = q.QuestionTags.Select(qt => qt.Tag.Name).ToList(),
                VoteCount = _db.Votes.Where(v => v.TargetType == VoteTarget.Question && v.TargetId == q.Id && v.VoteType == VoteType.Upvote).Count()
                            - _db.Votes.Where(v => v.TargetType == VoteTarget.Question && v.TargetId == q.Id && v.VoteType == VoteType.Downvote).Count(),
                AnswerCount = _db.Answers.Count(a => a.QuestionId == q.Id),
                HasAcceptedAnswer = q.AcceptedAnswerId != null
            })
            .ToListAsync(cancellationToken);

        var paged = new PagedResult<QuestionSummaryDto>
        {
            Items = items,
            TotalCount = total,
            Page = request.Page,
            PageSize = request.PageSize
        };

        await _cacheService.SetAsync(cacheKey, paged, TimeSpan.FromMinutes(2), cancellationToken);

        return Result<PagedResult<QuestionSummaryDto>>.Success(paged);
    }
}
