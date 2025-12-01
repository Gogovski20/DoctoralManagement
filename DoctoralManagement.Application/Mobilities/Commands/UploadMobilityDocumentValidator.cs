using FluentValidation;

namespace DoctoralManagement.Application.Mobilities.Commands
{
    public class UploadMobilityDocumentValidator : AbstractValidator<UploadMobilityDocumentCommand>
    {
        public UploadMobilityDocumentValidator()
        {
            RuleFor(x => x.MobilityId)
                .GreaterThan(0);

            RuleFor(x => x.File)
                .NotNull().WithMessage("File is required.")
                .Must(f => f.Length > 0).WithMessage("File cannot be empty.")
                .Must(f => f.Length <= 5 * 1024 * 1024)
                    .WithMessage("File must be less than 5MB.");

            RuleFor(x => x.FileName)
                .NotEmpty()
                .MaximumLength(255);

            RuleFor(x => x.Type)
                .IsInEnum();
        }
    }
}
