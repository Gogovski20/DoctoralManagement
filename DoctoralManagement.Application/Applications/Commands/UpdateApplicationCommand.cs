using MediatR;

namespace DoctoralManagement.Application.Applications.Commands
{
    public class UpdateApplicationCommand : IRequest<UpdateApplicationResponse>
    {
        public int Id { get; set; }
        public int? PreferredMentorId { get; set; }
        public string MotivationLetter { get; set; } = string.Empty;
        public string ResearchProposal { get; set; } = string.Empty;
        public string EnglishCertificatePath { get; set; } = string.Empty;
    }
}
