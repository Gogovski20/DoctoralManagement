namespace DoctoralManagement.Application.Students.Queries
{
    public class GetAllStudentsResponse
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string IndexNumber { get; set; } = string.Empty;
        public int TotalCredits { get; set; }
    }
}
