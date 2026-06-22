using FluentValidation;

namespace StackOverflowLite.Application.Features.Answers;

public class AcceptAnswerCommandValidator : AbstractValidator<AcceptAnswerCommand>
{
    public AcceptAnswerCommandValidator()
    {
        RuleFor(x => x.QuestionId).NotEmpty();
        RuleFor(x => x.AnswerId).NotEmpty().When(x => x.AnswerId.HasValue);
    }
}