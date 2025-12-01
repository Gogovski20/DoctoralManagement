using FluentValidation;

namespace DoctoralManagement.Application.ActivityDocuments
{
    public class DownloadActivityDocumentValidator : AbstractValidator<DownloadActivityDocumentQuery>
    {
        public DownloadActivityDocumentValidator()
        {
            RuleFor(x => x.DocumentId)
                .GreaterThan(0);

            RuleFor(x => x.ActivityId)
                .GreaterThan(0);

            RuleFor(x => x.ActivityType)
                .IsInEnum();
        }
    }
}
