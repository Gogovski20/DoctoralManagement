namespace DoctoralManagement.Domain.Entities
{
    public class Mentor
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public int MaxStudents { get; set; } = 5; // Default maximum number of students a mentor can supervise
        public bool IsActive { get; set; } = true; // Indicates if the mentor is currently active
        public List<string> ResearchAreas { get; set; } = new(); // Areas of expertise

        // Navigation
        public ICollection<DoctoralProject> DoctoralProjects { get; set; } = new List<DoctoralProject>();
        public ICollection<ProgramMentor> DoctoralPrograms { get; set; } = new List<ProgramMentor>();
    }
}
