using FluentValidation;

namespace DoctoralManagement.Application.Students.Commands
{
    public class DeleteStudentValidator : AbstractValidator<DeleteStudentCommand>
    {
        public DeleteStudentValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0);
        }
    }
}
