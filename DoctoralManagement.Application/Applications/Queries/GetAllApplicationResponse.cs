using DoctoralManagement.Domain.Entities;

namespace DoctoralManagement.Application.Applications.Queries
{
    public class GetAllApplicationResponse
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public string StudentEmail { get; set; } = string.Empty;
        public int DoctoralProgramId { get; set; }
        public string ProgramName { get; set; } = string.Empty;
        public string ScientificArea { get; set; } = string.Empty;
        public ApplicationStatus ApplicationStatus { get; set; }
        public DateTime ApplicationDate { get; set; }
        public DateTime? DecisionDate { get; set; }
        public bool MeetsGradeRequirements { get; set; }
    }
}
