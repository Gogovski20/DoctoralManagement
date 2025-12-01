using FluentValidation;

namespace DoctoralManagement.Application.ConferenceParticipations.Commands
{
    public class AddConferenceParticipationValidator : AbstractValidator<AddConferenceParticipationCommand>
    {
        public AddConferenceParticipationValidator()
        {
            RuleFor(x => x.StudentId)
                .GreaterThan(0);

            RuleFor(x => x.ConferenceName)
                .NotEmpty()
                .MaximumLength(200);

            RuleFor(x => x.Date)
                .LessThanOrEqualTo(DateTime.UtcNow.AddYears(1)); // adjust if needed

            RuleFor(x => x.Role)
                .NotEmpty()
                .MaximumLength(100);

            RuleFor(x => x.PossibleEctsCredits)
                .GreaterThanOrEqualTo(0)
                .LessThanOrEqualTo(30);
        }
    }
}
