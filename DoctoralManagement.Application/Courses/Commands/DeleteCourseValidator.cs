using FluentValidation;

namespace DoctoralManagement.Application.Courses.Commands
{
    public class DeleteCourseValidator : AbstractValidator<DeleteCourseCommand>
    {
        public DeleteCourseValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0);
        }
    }
}
