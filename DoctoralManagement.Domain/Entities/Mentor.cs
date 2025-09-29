namespace DoctoralManagement.Domain.Entities
{
    public class Mentor
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        // Navigation
        public ICollection<DoctoralProject> DoctoralProjects { get; set; } = new List<DoctoralProject>();
    }
}
