using FluentValidation;

namespace DoctoralManagement.Application.Publications.Commands
{
    public class ReviewPublicationValidator : AbstractValidator<ReviewPublicationCommand>
    {
        public ReviewPublicationValidator()
        {
            RuleFor(x => x.PublicationId)
                .GreaterThan(0);

            RuleFor(x => x.ReviewComments)
                .MaximumLength(1000);
        }
    }
}
