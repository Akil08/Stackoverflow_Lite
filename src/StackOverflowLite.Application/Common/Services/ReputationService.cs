using StackOverflowLite.Domain.Entities;
using StackOverflowLite.Domain.Enums;

namespace StackOverflowLite.Application.Common.Services;

public class ReputationService
{
    public void ApplyReputationChange(ApplicationUser author, VoteTarget targetType, VoteType voteType, bool isReversal)
    {
        var delta = GetDelta(targetType, voteType);

        if (isReversal)
        {
            delta = -delta;
        }

        author.Reputation = Math.Max(0, author.Reputation + delta);
    }

    private static int GetDelta(VoteTarget targetType, VoteType voteType)
    {
        return (targetType, voteType) switch
        {
            (VoteTarget.Question, VoteType.Upvote) => 5,
            (VoteTarget.Question, VoteType.Downvote) => -1,
            (VoteTarget.Answer, VoteType.Upvote) => 10,
            (VoteTarget.Answer, VoteType.Downvote) => -2,
            _ => 0
        };
    }
}