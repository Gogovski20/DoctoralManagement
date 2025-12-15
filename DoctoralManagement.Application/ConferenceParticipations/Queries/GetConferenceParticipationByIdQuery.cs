using DoctoralManagement.Application.Dtos;
using MediatR;

namespace DoctoralManagement.Application.ConferenceParticipations.Queries
{
    public class GetConferenceParticipationByIdQuery : IRequest<GetConferenceParticipationByIdResponse>
    {
        public int ConferenceId { get; set; }
    }

    public class GetConferenceParticipationByIdResponse
    {
        public int Id { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public string ConferenceName { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public string Role { get; set; } = string.Empty;
        public bool IsInternational { get; set; }
        public bool IsApproved { get; set; }
        public ActivityDocumentDto? Document { get; set; }   
    }
}
