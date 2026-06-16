using StackOverflowLite.Domain.Entities;

namespace StackOverflowLite.Application.Abstractions.Persistence;

public interface IApplicationDbContext
{
    IQueryable<Question> Questions { get; }

    IQueryable<Answer> Answers { get; }

    IQueryable<Vote> Votes { get; }

    IQueryable<Tag> Tags { get; }

    IQueryable<QuestionTag> QuestionTags { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}