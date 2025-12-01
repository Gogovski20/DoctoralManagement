using FluentValidation;

namespace DoctoralManagement.Application.ThesisDefenses
{
    public class ReviewThesisDocumentValidator : AbstractValidator<ReviewThesisDocumentCommand>
    {
        public ReviewThesisDocumentValidator()
        {
            RuleFor(x => x.DocumentId)
                .GreaterThan(0);

            RuleFor(x => x.NewStatus)
                .IsInEnum();

            RuleFor(x => x.ReviewComment)
                .MaximumLength(2000);
        }
    }
}
