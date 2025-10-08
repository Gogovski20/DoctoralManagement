using MediatR;

namespace DoctoralManagement.Application.Applications.Commands
{
    public class SubmitApplicationCommand : IRequest<SubmitApplicationResponse>
    {
        public int StudentId { get; set; }
        public int DoctoralProgramId { get; set; }
        public int? PreferredMentorId { get; set; }
        public string MotivationLetter { get; set; } = string.Empty;
        public string ResearchProposal { get; set; } = string.Empty;
        public string EnglishCertificatePath { get; set; } = string.Empty;
    }
}
