using FluentValidation;

namespace DoctoralManagement.Application.Courses.Commands
{
    public class UpdateCourseValidator : AbstractValidator<UpdateCourseCommand>
    {
        public UpdateCourseValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0);

            RuleFor(x => x.Code)
                .NotEmpty()
                .MaximumLength(15);

            RuleFor(x => x.Title)
                .NotEmpty()
                .MaximumLength(100);

            RuleFor(x => x.EctsCredits)
                .NotEmpty()
                .InclusiveBetween(0, 6);

            RuleFor(x => x.InstructorName)
                .NotEmpty()
                .MaximumLength(50);

            RuleFor(x => x.Semester)
                .NotEmpty()
                .InclusiveBetween(0, 6);

            RuleFor(x => x.Description)
                .NotEmpty()
                .MaximumLength(1000);
        }
    }
}
