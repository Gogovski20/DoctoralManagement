using FluentValidation;

namespace DoctoralManagement.Application.Students.Commands
{
    public class UpdateStudentValidator : AbstractValidator<UpdateStudentCommand>
    {
        public UpdateStudentValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0);

            RuleFor(x => x.FullName)
                .NotEmpty()
                .MaximumLength(200);

            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress()
                .MaximumLength(200);

            RuleFor(x => x.GPA)
                .InclusiveBetween(8m, 10m);

            RuleFor(x => x.EnglishCertificate)
                .MaximumLength(200);

            RuleFor(x => x.StudentStatus)
                .IsInEnum();
        }
    }
}
