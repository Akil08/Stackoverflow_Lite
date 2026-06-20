using FluentValidation;

namespace StackOverflowLite.Application.Features.Answers;

public class CreateAnswerCommandValidator : AbstractValidator<CreateAnswerCommand>
{
    public CreateAnswerCommandValidator()
    {
        RuleFor(x => x.Content).NotEmpty().MinimumLength(10);
    }
}