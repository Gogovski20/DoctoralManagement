using FluentValidation;

namespace DoctoralManagement.Application.DoctoralProjects.Commands
{
    public class UpdateDoctoralProjectValidator : AbstractValidator<UpdateDoctoralProjectCommand>
    {
        public UpdateDoctoralProjectValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0);

            RuleFor(x => x.Title)
                .NotEmpty()
                .MaximumLength(300);

            RuleFor(x => x.ResearchArea)
                .NotEmpty()
                .MaximumLength(300);

            RuleFor(x => x.EctsCredits)
                .GreaterThanOrEqualTo(0);

            RuleFor(x => x.MentorId)
                .GreaterThan(0);
        }
    }
}
