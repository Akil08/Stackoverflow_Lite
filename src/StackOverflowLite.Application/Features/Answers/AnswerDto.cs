namespace StackOverflowLite.Application.Features.Answers;

public class AnswerDto
{
    public Guid Id { get; set; }

    public string Content { get; set; } = string.Empty;

    public string AuthorName { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public bool IsAccepted { get; set; }

    public int VoteCount { get; set; }
}