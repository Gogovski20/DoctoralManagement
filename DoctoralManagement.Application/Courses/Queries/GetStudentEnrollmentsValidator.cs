using FluentValidation;

namespace DoctoralManagement.Application.Courses.Queries
{
    public class GetStudentEnrollmentsValidator : AbstractValidator<GetStudentEnrollmentsQuery>
    {
        public GetStudentEnrollmentsValidator()
        {
            RuleFor(x => x.StudentId)
                .GreaterThan(0);
        }
    }
}
