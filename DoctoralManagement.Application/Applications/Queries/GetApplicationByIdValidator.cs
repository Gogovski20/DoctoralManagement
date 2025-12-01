using FluentValidation;

namespace DoctoralManagement.Application.Applications.Queries
{
    public class GetApplicationByIdValidator : AbstractValidator<GetApplicationByIdQuery>
    {
        public GetApplicationByIdValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0);
        }
    }
}
