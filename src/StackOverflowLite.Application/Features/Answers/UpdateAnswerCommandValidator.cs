using FluentValidation;

namespace StackOverflowLite.Application.Features.Answers;

public class UpdateAnswerCommandValidator : AbstractValidator<UpdateAnswerCommand>
{
    public UpdateAnswerCommandValidator()
    {
        RuleFor(x => x.Content).NotEmpty().MinimumLength(10);
    }
}