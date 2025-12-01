using FluentValidation;

namespace DoctoralManagement.Application.ConferenceParticipations.Commands
{
    public class UpdateConferenceParticipationValidator : AbstractValidator<UpdateConferenceParticipationCommand>
    {
        public UpdateConferenceParticipationValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0);

            RuleFor(x => x.ConferenceName)
                .NotEmpty()
                .MaximumLength(200);

            RuleFor(x => x.Date)
                .LessThanOrEqualTo(DateTime.UtcNow.AddYears(1)); // adjust if needed

            RuleFor(x => x.Role)
                .NotEmpty()
                .MaximumLength(100);

            RuleFor(x => x.EctsCredits)
                .GreaterThanOrEqualTo(0)
                .LessThanOrEqualTo(30);
        }
    }
}
