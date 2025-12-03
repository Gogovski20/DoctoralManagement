using FluentValidation;

namespace DoctoralManagement.Application.Mobilities.Queries
{
    public class GetMobilityByIdValidator : AbstractValidator<GetMobilityByIdQuery>
    {
        public GetMobilityByIdValidator()
        {
            RuleFor(x => x.MobilityId)
                .GreaterThan(0);
        }
    }
}
