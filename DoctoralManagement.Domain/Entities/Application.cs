namespace DoctoralManagement.Domain.Entities
{
    public class Application
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public int DoctoralProgramId { get; set; }
        public int? PrefferedMentorId { get; set; }
        public ApplicationStatus ApplicationStatus { get; set; }
        public DateTime ApplicationDate { get; set; }
        public DateTime? DecisionDate { get; set; }

        // UKIM Requirements
        public string MotivationLetter { get; set; } = string.Empty;
        public string ResearchProposal { get; set; } = string.Empty;
        public string EnglishCertificatePath { get; set; } = string.Empty;
        public bool MeetsGradeRequirements { get; set; }
        public bool HasRequiredPublications { get; set; }

        // Navigation
        public Student Student { get; set; } = null!;
        public DoctoralProgram DoctoralProgram { get; set; } = null!;
        public Mentor? PrefferedMentor { get; set; }
    }
}
