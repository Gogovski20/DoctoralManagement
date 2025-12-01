using FluentValidation;

namespace DoctoralManagement.Application.DoctoralProjects.Commands
{
    public class SubmitDoctoralProjectValidator : AbstractValidator<SubmitDoctoralProjectCommand>
    {
        public SubmitDoctoralProjectValidator()
        {
            RuleFor(x => x.ProjectId)
                .GreaterThan(0);
        }
    }
}
