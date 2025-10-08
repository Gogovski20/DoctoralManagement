using DoctoralManagement.Domain.Entities;

namespace DoctoralManagement.Application.Applications.Queries
{
    public class GetProgramApplicationsResponse
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public string StudentEmail { get; set; } = string.Empty;
        public decimal StudentGPA { get; set; }
        public ApplicationStatus ApplicationStatus { get; set; }
        public DateTime ApplicationDate { get; set; }
        public bool MeetsGradeRequirements { get; set; }
    }
}
