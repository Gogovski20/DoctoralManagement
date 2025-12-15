using DoctoralManagement.Application.Dtos;
using MediatR;

namespace DoctoralManagement.Application.ConferenceParticipations.Queries
{
    public class GetAllConferenceParticipationsQuery : IRequest<IEnumerable<GetAllConferenceParticipationsResponse>>
    {
    }

    public class GetAllConferenceParticipationsResponse
    {
        public int Id { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public string ConferenceName { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public string Role { get; set; } = string.Empty;
        public bool IsInternational { get; set; }
        public DocumentDto? Document { get; set; }
    }
}
