namespace StackOverflowLite.Domain.Entities;

public class ApplicationUser
{
    public Guid Id { get; set; }

    public string UserName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public int Reputation { get; set; } = 0;

    public ICollection<Question> Questions { get; set; } = new List<Question>();

    public ICollection<Answer> Answers { get; set; } = new List<Answer>();

    public ICollection<Vote> Votes { get; set; } = new List<Vote>();
}