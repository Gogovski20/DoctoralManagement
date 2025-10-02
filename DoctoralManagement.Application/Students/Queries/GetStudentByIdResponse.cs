namespace DoctoralManagement.Application.Students.Queries
{
    public class GetStudentByIdResponse
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string IndexNumber { get; set; } = string.Empty;
        public DateTime EnrollmentDate { get; set; }
        public int TotalCredits { get; set; }
    }
}
