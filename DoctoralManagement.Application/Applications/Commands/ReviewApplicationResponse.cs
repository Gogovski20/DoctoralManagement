using DoctoralManagement.Domain.Entities;

namespace DoctoralManagement.Application.Applications.Commands
{
    public class ReviewApplicationResponse
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public int DoctoralProgramId { get; set; }
        public ApplicationStatus ApplicationStatus { get; set; }
        public string ReviewComments { get; set; } = string.Empty;
        public bool HasRequiredPublications { get; set; }
        public DateTime? DecisionDate { get; set; }
    }
}
