using FluentValidation;

namespace DoctoralManagement.Application.DoctoralProjects.Commands
{
    public class ReviewDoctoralProjectValidator : AbstractValidator<ReviewDoctoralProjectCommand>
    {
        public ReviewDoctoralProjectValidator()
        {
            RuleFor(x => x.ProjectId)
                .GreaterThan(0);

            RuleFor(x => x.NewStatus)
                .IsInEnum();

            RuleFor(x => x.CommitteeNotes)
                .MaximumLength(2000);

            RuleFor(x => x.DocumentStatus)
                .IsInEnum()
                .When(x => x.DocumentStatus.HasValue);

            RuleFor(x => x.ReviewComment)
                .MaximumLength(2000);
        }
    }
}
