using DoctoralManagement.Domain.Entities;

namespace DoctoralManagement.Application.Students.Queries
{
    public class GetAllStudentsResponse
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string IndexNumber { get; set; } = string.Empty;
        public decimal GPA { get; set; }
        public StudentStatus StudentStatus { get; set; }
        public int TotalCredits { get; set; }
    }
}
