using DoctoralManagement.Domain.Entities;

namespace DoctoralManagement.Application.Students.Queries
{
    public class GetStudentByIdResponse
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string IndexNumber { get; set; } = string.Empty;
        public DateTime EnrollmentDate { get; set; }
        public decimal GPA { get; set; }
        public string EnglishCertificate { get; set; } = string.Empty;
        public StudentStatus StudentStatus { get; set; }
        public int TotalCreditsFromBachelor { get; set; }
        public int TotalCreditsFromMaster { get; set; }
        public int TotalCredits { get; set; }
    }
}
