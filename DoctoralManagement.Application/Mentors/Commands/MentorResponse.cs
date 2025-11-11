namespace DoctoralManagement.Application.Mentors.Commands
{
    public class MentorResponse
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public int MaxStudents { get; set; }
        public bool IsActive { get; set; }
        public List<string> ResearchAreas { get; set; } = new();
    }
}
