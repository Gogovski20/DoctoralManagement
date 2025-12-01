using FluentValidation;

namespace DoctoralManagement.Application.Mentors.Commands
{
    public class UpdateMentorValidator : AbstractValidator<UpdateMentorCommand>
    {
        public UpdateMentorValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0);

            RuleFor(x => x.FullName)
                .NotEmpty()
                .MaximumLength(200);

            RuleFor(x => x.Department)
                .NotEmpty()
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
