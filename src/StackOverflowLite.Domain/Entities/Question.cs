namespace StackOverflowLite.Domain.Entities;

public class Question
{
    public Guid Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public int ViewCount { get; set; } = 0;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public Guid AuthorId { get; set; }

    public ApplicationUser Author { get; set; } = null!;

    public ICollection<Answer> Answers { get; set; } = new List<Answer>();

    public ICollection<Vote> Votes { get; set; } = new List<Vote>();

    public ICollection<QuestionTag> QuestionTags { get; set; } = new List<QuestionTag>();

    public Guid? AcceptedAnswerId { get; set; }
}