using FluentValidation;

namespace DoctoralManagement.Application.Mentors.Queries
{
    public class GetMentorByIdValidator : AbstractValidator<GetMentorByIdQuery>
    {
        public GetMentorByIdValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0);
        }
    }
}
