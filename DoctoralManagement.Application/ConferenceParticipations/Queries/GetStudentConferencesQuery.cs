using DoctoralManagement.Application.Dtos;
using DoctoralManagement.Domain.Entities;
using MediatR;

namespace DoctoralManagement.Application.ConferenceParticipations.Queries
{
    public class GetStudentConferencesQuery : IRequest<IEnumerable<ConferenceParticipationResponse>>
    {
        public int StudentId { get; set; }
    }

    public class ConferenceParticipationResponse
    {
        public int Id { get; set; }
        public string ConferenceName { get; set; } = string.Empty;
        public System.DateTime Date { get; set; }
        public string Role { get; set; } = string.Empty;
        public bool IsInternational { get; set; }
        public DocumentDto? Document { get; set; }
    }
}
