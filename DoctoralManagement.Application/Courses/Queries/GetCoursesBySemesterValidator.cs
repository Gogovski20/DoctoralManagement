using FluentValidation;

namespace DoctoralManagement.Application.Courses.Queries
{
    public class GetCoursesBySemesterValidator : AbstractValidator<GetCoursesBySemesterQuery>
    {
        public GetCoursesBySemesterValidator()
        {
            RuleFor(x => x.Semester)
                .GreaterThan(0)
                .LessThanOrEqualTo(6); 
        }
    }
}
