using FluentValidation;

namespace DoctoralManagement.Application.Applications.Commands
{
    public class CreateApplicationValidator : AbstractValidator<CreateApplicationCommand>
    {
        public CreateApplicationValidator()
        {
            RuleFor(x => x.StudentId)
                .GreaterThan(0);

            RuleFor(x => x.DoctoralProgramId)
                .GreaterThan(0);

            RuleFor(x => x.PreferredMentorId)
                .GreaterThan(0)
                .When(x => x.PreferredMentorId.HasValue);
        }
    }
}
