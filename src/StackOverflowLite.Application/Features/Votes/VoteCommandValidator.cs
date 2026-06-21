using FluentValidation;
using StackOverflowLite.Domain.Enums;

namespace StackOverflowLite.Application.Features.Votes;

public class VoteCommandValidator : AbstractValidator<VoteCommand>
{
    public VoteCommandValidator()
    {
        RuleFor(x => x.TargetId).NotEmpty();
        RuleFor(x => x.TargetType).Must(value => Enum.IsDefined(typeof(VoteTarget), value));
        RuleFor(x => x.VoteType).Must(value => Enum.IsDefined(typeof(VoteType), value));
    }
}