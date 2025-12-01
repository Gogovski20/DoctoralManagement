using FluentValidation;

namespace DoctoralManagement.Application.Mentors.Commands
{
    public class CreateMentorValidator : AbstractValidator<CreateMentorCommand>
    {
        public CreateMentorValidator()
        {
            RuleFor(x => x.FullName)
                .NotEmpty()
                .MaximumLength(200);

            RuleFor(x => x.Department)
                .NotEmpty()
                .MaximumLength(200);

            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress()
                .MaximumLength(200);

            RuleFor(x => x.Title)
                .NotEmpty()
                .MaximumLength(100);

            RuleFor(x => x.MaxStudents)
                .GreaterThan(0)
                .LessThanOrEqualTo(50);

            RuleForEach(x => x.ResearchAreas)
                .NotEmpty()
                .MaximumLength(200);
        }
    }
}
