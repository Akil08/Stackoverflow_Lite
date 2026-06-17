namespace StackOverflowLite.Application.Features.Auth;

public class ProfileDto
{
    public string UserName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public int Reputation { get; set; }

    public int TotalQuestions { get; set; }

    public int TotalAnswers { get; set; }
}