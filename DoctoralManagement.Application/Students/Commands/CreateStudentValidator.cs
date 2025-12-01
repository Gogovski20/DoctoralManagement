using FluentValidation;

namespace DoctoralManagement.Application.Students.Commands
{
    public class CreateStudentValidator : AbstractValidator<CreateStudentCommand>
    {
        public CreateStudentValidator()
        {
            RuleFor(x => x.FullName)
                .NotEmpty()
                .MaximumLength(200);

            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress()
                .MaximumLength(200);

            RuleFor(x => x.IndexNumber)
                .NotEmpty()
                .MaximumLength(50);

            RuleFor(x => x.EnrollmentDate)
                .LessThanOrEqualTo(DateTime.UtcNow.AddDays(1));

            RuleFor(x => x.GPA)
                .InclusiveBetween(8m, 10m);

            RuleFor(x => x.EnglishCertificate)
                .MaximumLength(200);

            RuleFor(x => x.TotalCreditsFromBachelor)
                .GreaterThanOrEqualTo(0);

            RuleFor(x => x.TotalCreditsFromMaster)
                .GreaterThanOrEqualTo(0);
        }
    }
}
