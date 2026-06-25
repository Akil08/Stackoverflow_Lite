using MediatR;
using Microsoft.EntityFrameworkCore;
using StackOverflowLite.Application.Common.Interfaces;
using StackOverflowLite.Application.Common.Models;
using StackOverflowLite.Domain.Entities;

namespace StackOverflowLite.Application.Features.Questions;

public class UpdateQuestionCommandHandler : IRequestHandler<UpdateQuestionCommand, Result<bool>>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUserService _currentUser;
    private readonly ICacheService _cacheService;

    public UpdateQuestionCommandHandler(IApplicationDbContext db, ICurrentUserService currentUser, ICacheService cacheService)
    {
        _db = db;
        _currentUser = currentUser;
        _cacheService = cacheService;
    }

    public async Task<Result<bool>> Handle(UpdateQuestionCommand request, CancellationToken cancellationToken)
    {
        if (!_currentUser.IsAuthenticated)
            return Result<bool>.Failure("Not authenticated");

        if (!Guid.TryParse(_currentUser.UserId, out var userId))
            return Result<bool>.Failure("Invalid user id");

        var question = await _db.Questions.Include(q => q.QuestionTags).FirstOrDefaultAsync(q => q.Id == request.QuestionId, cancellationToken);
        if (question == null) return Result<bool>.Failure("Question not found");

        if (question.AuthorId != userId) return Result<bool>.Failure("Forbidden");

        question.Title = request.Title;
        question.Description = request.Description;
        question.UpdatedAt = DateTime.UtcNow;

        // Replace tags
        var tags = request.Tags?.Select(t => t.Trim()).Where(t => !string.IsNullOrEmpty(t)).Distinct().Take(5).ToList() ?? new List<string>();

        var existingTags = await _db.Tags.Where(t => tags.Contains(t.Name)).ToListAsync(cancellationToken);
        var newTagNames = tags.Except(existingTags.Select(t => t.Name)).ToList();
        var newTags = newTagNames.Select(n => new Tag { Id = Guid.NewGuid(), Name = n }).ToList();
        foreach (var nt in newTags) _db.Tags.Add(nt);

        // remove old questiontags
        var oldQuestionTags = _db.QuestionTags.Where(qt => qt.QuestionId == question.Id);
        foreach (var oqt in oldQuestionTags) _db.QuestionTags.Remove(oqt);

        var allTags = existingTags.Concat(newTags).ToList();
        foreach (var tag in allTags)
        {
            _db.QuestionTags.Add(new QuestionTag { QuestionId = question.Id, TagId = tag.Id });
        }

        await _db.SaveChangesAsync(cancellationToken);
        await _cacheService.RemoveByPrefixAsync("questions:list", cancellationToken);

        return Result<bool>.Success(true);
    }
}
