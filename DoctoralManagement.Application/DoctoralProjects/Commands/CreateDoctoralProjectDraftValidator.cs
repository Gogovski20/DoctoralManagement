using FluentValidation;

namespace DoctoralManagement.Application.DoctoralProjects.Commands
{
    public class CreateDoctoralProjectDraftValidator : AbstractValidator<CreateDoctoralProjectDraftCommand>
    {
        public CreateDoctoralProjectDraftValidator()
        {
            RuleFor(x => x.StudentId)
                .GreaterThan(0);

            RuleFor(x => x.MentorId)
                .GreaterThan(0);

            RuleFor(x => x.Title)
                .NotEmpty()
                .MaximumLength(300);

            RuleFor(x => x.ResearchArea)
                .NotEmpty()
                .MaximumLength(300);

            RuleFor(x => x.ProposalDocumentPath)
                .MaximumLength(500);
        }
    }
}
