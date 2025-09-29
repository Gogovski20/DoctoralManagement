namespace DoctoralManagement.Domain.Entities
{
    public class Student
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string IndexNumber { get; set; } = string.Empty;
        public DateTime EnrollmentDate { get; set; }


        public int TotalCreditsFromBachelor { get; set; }
        public int TotalCreditsFromMaster { get; set; }
        public int TotalCredits => TotalCreditsFromBachelor + TotalCreditsFromMaster;

        // Navigation
        public ICollection<DoctoralProject> DoctoralProjects { get; set; } = new List<DoctoralProject>();
        public ICollection<Publication> Publications { get; set; } = new List<Publication>();
        public ICollection<Mobility> Mobilities { get; set; } = new List<Mobility>();
    }
}
