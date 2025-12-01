using FluentValidation;

namespace DoctoralManagement.Application.Courses.Commands
{
    public class CreateCourseValidator : AbstractValidator<CreateCourseCommand>
    {
        public CreateCourseValidator()
        {
            RuleFor(x => x.Code)
                .NotEmpty()
                .MaximumLength(50);

            RuleFor(x => x.Title)
                .NotEmpty()
                .MaximumLength(200);

            RuleFor(x => x.EctsCredits)
                .GreaterThan(0)
                .LessThanOrEqualTo(30);

            RuleFor(x => x.InstructorName)
                .NotEmpty()
                .MaximumLength(200);

            RuleFor(x => x.Semester)
                .GreaterThan(0)
                .LessThanOrEqualTo(6); 

            RuleFor(x => x.Description)
                .MaximumLength(2000);
        }
    }
}
