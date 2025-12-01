using FluentValidation;

namespace DoctoralManagement.Application.Courses.Commands
{
    public class EnrollStudentInCourseValidator : AbstractValidator<EnrollStudentInCourseCommand>
    {
        public EnrollStudentInCourseValidator()
        {
            RuleFor(x => x.StudentId)
                .GreaterThan(0);

            RuleFor(x => x.CourseId)
                .GreaterThan(0);
        }
    }
}
