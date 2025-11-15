using MediatR;

namespace DoctoralManagement.Application.DoctoralProjects.Commands
{
    public class UpdateDoctoralProjectCommand : IRequest<UpdateDoctoralProjectResponse>
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string ResearchArea { get; set; } = string.Empty;
        public int EctsCredits { get; set; }
        public int MentorId { get; set; } // change mentor if needed
        public string? ProposalDocumentPath { get; set; }
    }
}
