using FluentValidation;

namespace DoctoralManagement.Application.Publications.Queries
{
    public class GetPublicationByIdValidator : AbstractValidator<GetPublicationByIdQuery>
    {
        public GetPublicationByIdValidator()
        {
            RuleFor(x => x.PublicationId)
                .GreaterThan(0);
        }
    }
}
