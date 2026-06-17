namespace StackOverflowLite.Application.Features.Questions;

public class QuestionDetailDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string AuthorName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public List<string> Tags { get; set; } = new();
    public int VoteCount { get; set; }
    public int AnswerCount { get; set; }
    public Guid? AcceptedAnswerId { get; set; }
    public int ViewCount { get; set; }
}
