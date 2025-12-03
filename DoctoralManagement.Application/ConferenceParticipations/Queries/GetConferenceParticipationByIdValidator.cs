using FluentValidation;

namespace DoctoralManagement.Application.ConferenceParticipations.Queries
{
    public class GetConferenceParticipationByIdValidator : AbstractValidator<GetConferenceParticipationByIdQuery>
    {
        public GetConferenceParticipationByIdValidator()
        {
            RuleFor(x => x.ConferenceId)
                .GreaterThan(0);
        }
    }
}
