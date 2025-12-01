using FluentValidation;

namespace DoctoralManagement.Application.DoctoralProjects.Commands
{
    public class UploadDoctoralProjectProposalValidator : AbstractValidator<UploadDoctoralProjectProposalCommand>
    {
        public UploadDoctoralProjectProposalValidator()
        {
            RuleFor(x => x.DoctoralProjectId)
                .GreaterThan(0);

            RuleFor(x => x.File)
                .NotNull().WithMessage("File is required.")
                .Must(f => f.Length > 0).WithMessage("File cannot be empty.")
                .Must(f => f.Length <= 5 * 1024 * 1024)
                    .WithMessage("File must be less than 5MB.");

            RuleFor(x => x.FileName)
                .NotEmpty()
                .MaximumLength(255);

            RuleFor(x => x.DocumentType)
                .IsInEnum();
        }
    }
}
