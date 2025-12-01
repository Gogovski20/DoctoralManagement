using FluentValidation;

namespace DoctoralManagement.Application.ApplicationDocuments
{
    public class DownloadApplicationDocumentValidator : AbstractValidator<DownloadApplicationDocumentQuery>
    {
        public DownloadApplicationDocumentValidator()
        {
            RuleFor(x => x.ApplicationId)
                .GreaterThan(0);
            RuleFor(x => x.DocumentId)
                .GreaterThan(0);
        }
    }
}
