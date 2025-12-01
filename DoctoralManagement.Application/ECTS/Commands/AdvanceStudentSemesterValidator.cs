using FluentValidation;

namespace DoctoralManagement.Application.ECTS.Commands
{
    public class AdvanceStudentSemesterValidator : AbstractValidator<AdvanceStudentSemesterCommand>
    {
        public AdvanceStudentSemesterValidator()
        {
            RuleFor(x => x.StudentId)
                .GreaterThan(0);

            RuleFor(x => x.Semester)
                .GreaterThan(0)
                .LessThanOrEqualTo(6); 
        }
    }
}
