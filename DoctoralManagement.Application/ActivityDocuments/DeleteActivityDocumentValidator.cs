using FluentValidation;

namespace DoctoralManagement.Application.ActivityDocuments
{
    public class DeleteActivityDocumentValidator : AbstractValidator<DeleteActivityDocumentCommand>
    {
        public DeleteActivityDocumentValidator()
        {
            RuleFor(x => x.ActivityDocumentId)
                .GreaterThan(0);

            RuleFor(x => x.ActivityId)
                .GreaterThan(0);

            RuleFor(x => x.ActivityType)
                .IsInEnum();
        }
    }
}
