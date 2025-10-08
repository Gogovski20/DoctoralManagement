using DoctoralManagement.Domain.Entities;

namespace DoctoralManagement.Application.Applications.Commands
{
    public class SubmitApplicationResponse
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public int DoctoralProgramId { get; set; }
        public int? PreferredMentorId { get; set; }
        public string MotivationLetter { get; set; } = string.Empty;
        public string ResearchProposal { get; set; } = string.Empty;
        public string EnglishCertificatePath { get; set; } = string.Empty;
        public ApplicationStatus ApplicationStatus { get; set; }
        public DateTime ApplicationDate { get; set; }
        public bool MeetsGradeRequirements { get; set; }
        public bool HasRequiredPublications { get; set; }
    }
}
