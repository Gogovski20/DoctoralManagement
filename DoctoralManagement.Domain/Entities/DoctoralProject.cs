namespace DoctoralManagement.Domain.Entities
{
    public class DoctoralProject
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string ResearchArea { get; set; } = string.Empty;
        public int EctsCredits { get; set; }

        public int StudentId { get; set; }
        // Navigation
        public Student? Student { get; set; } 

        public int MentorId { get; set; }
        // Navigation
        public Mentor? Mentor { get; set; }
    }
}
