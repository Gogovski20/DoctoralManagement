namespace DoctoralManagement.Domain.Entities
{
    public class Student
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string IndexNumber { get; set; } = string.Empty;
        public DateTime EnrollmentDate { get; set; }
        public decimal GPA { get; set; }
        public string EnglishCertificate { get; set; } = string.Empty;
        public StudentStatus Status { get; set; } = StudentStatus.Active;


        public int TotalCreditsFromBachelor { get; set; }
        public int TotalCreditsFromMaster { get; set; }
        public int TotalCredits => TotalCreditsFromBachelor + TotalCreditsFromMaster;

        public int? DoctoralProgramId { get; set; }
        public DoctoralProgram? DoctoralProgram { get; set; }

        // Navigation
        public ICollection<DoctoralProject> DoctoralProjects { get; set; } = new List<DoctoralProject>();
        public ICollection<Publication> Publications { get; set; } = new List<Publication>();
        public ICollection<Mobility> Mobilities { get; set; } = new List<Mobility>();
        public ICollection<Application> Applications { get; set; } = new List<Application>();
        public ECTSTracking? ECTSTracking { get; set; }
    }
}
