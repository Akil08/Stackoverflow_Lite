using StackOverflowLite.Domain.Enums;

namespace StackOverflowLite.Domain.Entities;

public class Vote
{
    public Guid Id { get; set; }

    public VoteType VoteType { get; set; }

    public VoteTarget TargetType { get; set; }

    public Guid TargetId { get; set; }

    public Guid UserId { get; set; }

    public ApplicationUser User { get; set; } = null!;

    public DateTime CreatedAt { get; set; }
}