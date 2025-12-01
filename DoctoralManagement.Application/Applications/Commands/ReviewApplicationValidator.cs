using FluentValidation;

namespace DoctoralManagement.Application.Applications.Commands
{
    public class ReviewApplicationValidator : AbstractValidator<ReviewApplicationCommand>
    {
        public ReviewApplicationValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0);

            RuleFor(x => x.NewStatus)
                .IsInEnum();

            RuleFor(x => x.ReviewComments)
                .MaximumLength(1000);
        }
    }
}
