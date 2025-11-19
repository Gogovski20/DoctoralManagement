using MediatR;

namespace DoctoralManagement.Application.ConferenceParticipations.Commands
{
    public class DeleteConferenceParticipationCommand : IRequest<bool>
    {
        public int Id { get; set; }
    }
}
