using DoctoralManagement.Application.Dtos;
using MediatR;

namespace DoctoralManagement.Application.DoctoralProjects.Queries
{
    public class GetDoctoralProjectByIdQuery : IRequest<GetDoctoralProjectByIdResponse>
    {
        public int DoctoralProjectId { get; set; }
    }

    public class GetDoctoralProjectByIdResponse
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string ResearchArea { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string StudentName { get; set; } = string.Empty;
        public string MentorName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? SubmittedAt { get; set; }
        public List<ActivityDocumentDto> Documents { get; set; } = new List<ActivityDocumentDto>();
    }
}
