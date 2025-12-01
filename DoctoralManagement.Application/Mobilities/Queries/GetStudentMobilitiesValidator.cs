using FluentValidation;

namespace DoctoralManagement.Application.Mobilities.Queries
{
    public class GetStudentMobilitiesValidator : AbstractValidator<GetStudentMobilitiesQuery>
    {
        public GetStudentMobilitiesValidator()
        {
            RuleFor(x => x.StudentId)
                .GreaterThan(0);
        }
    }
}
