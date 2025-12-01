using FluentValidation;

namespace DoctoralManagement.Application.Mobilities.Commands
{
    public class ReviewMobilityValidator : AbstractValidator<ReviewMobilityCommand>
    {
        public ReviewMobilityValidator()
        {
            RuleFor(x => x.MobilityId)
                .GreaterThan(0);

            RuleFor(x => x.ReviewComments)
                .MaximumLength(1000);
        }
    }
}
