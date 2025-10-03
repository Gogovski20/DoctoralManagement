using DoctoralManagement.Domain.Entities;

namespace DoctoralManagement.Application.Students.Commands
{
    public class UpdateStudentResponse
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string IndexNumber { get; set; } = string.Empty;
        public DateTime EnrollmentDate { get; set; }
        public decimal GPA { get; set; }
        public string EnglishCertificate { get; set; } = string.Empty;
        public StudentStatus StudentStatus { get; set; }
        public int TotalCredits { get; set; }
    }
}
