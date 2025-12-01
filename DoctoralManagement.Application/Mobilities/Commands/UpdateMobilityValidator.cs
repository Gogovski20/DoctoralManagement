using FluentValidation;

namespace DoctoralManagement.Application.Mobilities.Commands
{
    public class UpdateMobilityValidator : AbstractValidator<UpdateMobilityCommand>
    {
        public UpdateMobilityValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0);

            RuleFor(x => x.Institution)
                .NotEmpty()
                .MaximumLength(200);

            RuleFor(x => x.Country)
                .NotEmpty()
                .MaximumLength(100);

            RuleFor(x => x.StartDate)
                .LessThan(x => x.EndDate);

            RuleFor(x => x.EndDate)
                .GreaterThan(x => x.StartDate);

            RuleFor(x => x.EctsCredits)
                .GreaterThanOrEqualTo(0)
                .LessThanOrEqualTo(30);
        }
    }
}
