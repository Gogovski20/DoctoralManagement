using FluentValidation;

namespace DoctoralManagement.Application.ConferenceParticipations.Commands
{
    public class DeleteConferenceParticipationValidator : AbstractValidator<DeleteConferenceParticipationCommand>
    {
        public DeleteConferenceParticipationValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0);
        }
    }
}
