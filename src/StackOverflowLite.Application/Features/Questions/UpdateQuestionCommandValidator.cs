using FluentValidation;

namespace StackOverflowLite.Application.Features.Questions;

public class UpdateQuestionCommandValidator : AbstractValidator<UpdateQuestionCommand>
{
    public UpdateQuestionCommandValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(300);
        RuleFor(x => x.Description).NotEmpty();
        RuleFor(x => x.Tags).Must(t => t == null || t.Count <= 5).WithMessage("At most 5 tags are allowed");
    }
}
