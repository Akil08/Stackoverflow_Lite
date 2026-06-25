using MediatR;
using Microsoft.EntityFrameworkCore;
using StackOverflowLite.Application.Common.Interfaces;
using StackOverflowLite.Application.Common.Models;
using StackOverflowLite.Domain.Entities;

namespace StackOverflowLite.Application.Features.Questions;

public class CreateQuestionCommandHandler : IRequestHandler<CreateQuestionCommand, Result<Guid>>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;
    private readonly ICacheService _cacheService;

    public CreateQuestionCommandHandler(IApplicationDbContext db, ICurrentUserService currentUser, ICacheService cacheService)
    {
        _db = db;
        _currentUser = currentUser;
        _cacheService = cacheService;
    }

    public async Task<Result<Guid>> Handle(CreateQuestionCommand request, CancellationToken cancellationToken)
    {
        if (!_currentUser.IsAuthenticated)
            return Result<Guid>.Failure("Not authenticated");

        if (!Guid.TryParse(_currentUser.UserId, out var authorId))
            return Result<Guid>.Failure("Invalid user id");

        var tags = request.Tags?.Select(t => t.Trim()).Where(t => !string.IsNullOrEmpty(t)).Distinct().Take(5).ToList() ?? new List<string>();

        var existingTags = await _db.Tags.Where(t => tags.Contains(t.Name)).ToListAsync(cancellationToken);

        var newTagNames = tags.Except(existingTags.Select(t => t.Name)).ToList();
        var newTags = newTagNames.Select(n => new Tag { Id = Guid.NewGuid(), Name = n }).ToList();

        foreach (var nt in newTags) _db.Tags.Add(nt);

        var question = new Question
        {
            Id = Guid.NewGuid(),
            Title = request.Title,
            Description = request.Description,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            AuthorId = authorId,
            ViewCount = 0
        };

        _db.Questions.Add(question);

        var allTags = existingTags.Concat(newTags).ToList();
        foreach (var tag in allTags)
        {
            _db.QuestionTags.Add(new QuestionTag { QuestionId = question.Id, TagId = tag.Id });
        }

        await _db.SaveChangesAsync(cancellationToken);
        await _cacheService.RemoveByPrefixAsync("questions:list", cancellationToken);

        return Result<Guid>.Success(question.Id);
    }
}
