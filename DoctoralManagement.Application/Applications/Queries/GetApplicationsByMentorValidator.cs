using FluentValidation;

namespace DoctoralManagement.Application.Applications.Queries
{
    public class GetApplicationsByMentorValidator : AbstractValidator<GetApplicationsByMentorQuery>
    {
        public GetApplicationsByMentorValidator()
        {
            RuleFor(x => x.PreferredMentorId)
                .GreaterThan(0);
        }
    }
}
