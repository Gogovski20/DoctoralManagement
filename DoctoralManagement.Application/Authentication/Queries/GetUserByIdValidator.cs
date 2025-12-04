using FluentValidation;

namespace DoctoralManagement.Application.Authentication.Queries
{
    public class GetUserByIdValidator : AbstractValidator<GetUserByIdQuery>
    {
        public GetUserByIdValidator()
        {
            RuleFor(x => x.UserId)
                .GreaterThan(0);
        }
    }
}
