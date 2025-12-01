using FluentValidation;

namespace DoctoralManagement.Application.ApplicationDocuments
{
    public class DeleteApplicationDocumentValidator : AbstractValidator<DeleteApplicationDocumentCommand>
    {
        public DeleteApplicationDocumentValidator()
        {
            RuleFor(x => x.ApplicationId)
                .GreaterThan(0);

            RuleFor(x => x.DocumentId)
                .GreaterThan(0);
        }
    }
}
