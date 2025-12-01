using FluentValidation;

namespace DoctoralManagement.Application.DoctoralProjects.Commands
{
    public class CompleteDoctoralProjectValidator : AbstractValidator<CompleteDoctoralProjectCommand>
    {
        public CompleteDoctoralProjectValidator()
        {
            RuleFor(x => x.ProjectId)
                .GreaterThan(0);

            RuleFor(x => x.FinalReportNotes)
                .MaximumLength(2000);
        }
    }
}
