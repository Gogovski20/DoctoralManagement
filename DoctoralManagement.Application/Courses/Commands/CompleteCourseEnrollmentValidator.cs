using FluentValidation;

namespace DoctoralManagement.Application.Courses.Commands
{
    public class CompleteCourseEnrollmentValidator : AbstractValidator<CompleteCourseEnrollmentCommand>
    {
        public CompleteCourseEnrollmentValidator()
        {
            RuleFor(x => x.StudentId)
                .GreaterThan(0);

            RuleFor(x => x.EnrollmentId)
                .GreaterThan(0);

            RuleFor(x => x.Grade)
                .InclusiveBetween(5m, 10m); 
        }
    }
}
