using DoctoralManagement.Domain.Entities;

namespace DoctoralManagement.Application.Applications.Queries
{
    public class GetStudentApplicationsResponse
    {
        public int Id { get; set; }
        public int DoctoralProgramId { get; set; }
        public string ProgramName { get; set; } = string.Empty;
        public string Faculty { get; set; } = string.Empty;
        public ApplicationStatus ApplicationStatus { get; set; }
        public DateTime ApplicationDate { get; set; }
        public DateTime? DecisionDate { get; set; }
    }
}
