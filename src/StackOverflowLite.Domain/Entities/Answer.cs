namespace StackOverflowLite.Domain.Entities;

public class Answer
{
    public Guid Id { get; set; }

    public string Content { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public bool IsAccepted { get; set; } = false;

    public Guid QuestionId { get; set; }

    public Question Question { get; set; } = null!;

    public Guid AuthorId { get; set; }

    public ApplicationUser Author { get; set; } = null!;

    public ICollection<Vote> Votes { get; set; } = new List<Vote>();
}