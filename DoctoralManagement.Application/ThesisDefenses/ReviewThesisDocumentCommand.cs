using DoctoralManagement.Domain.Entities;
using MediatR;

namespace DoctoralManagement.Application.ThesisDefenses
{
    public class ReviewThesisDocumentCommand : IRequest<ReviewThesisDocumentResponse>
    {
        public int DocumentId { get; set; }
        public DocumentStatus NewStatus { get; set; }  
        public string? ReviewComment { get; set; }
    }

    public class ReviewThesisDocumentResponse
    {
        public int DocumentId { get; set; }
        public string DocumentStatus { get; set; } = string.Empty;
        public string? ReviewComment { get; set; }
        public string ProjectStatus { get; set; } = string.Empty;
        public int UpdatedECTS { get; set; }
    }
}
