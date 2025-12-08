using DoctoralManagement.Domain.Entities;
using MediatR;

namespace DoctoralManagement.Application.Applications.Queries
{
    public class GetApplicationsByMentorQuery : IRequest<IEnumerable<GetApplicationsByMentorResponse>>
    {
        public int PreferredMentorId { get; set; }
    }

    public class GetApplicationsByMentorResponse
    {
        public int Id { get; set; }
        public int DoctoralProgramId { get; set; }
        public string MentorName { get; set; } = string.Empty;
        public string ProgramName { get; set; } = string.Empty;
        public string Faculty { get; set; } = string.Empty;
        public ApplicationStatus ApplicationStatus { get; set; }
        public DateTime ApplicationDate { get; set; }
        public DateTime? DecisionDate { get; set; }
    }
}
