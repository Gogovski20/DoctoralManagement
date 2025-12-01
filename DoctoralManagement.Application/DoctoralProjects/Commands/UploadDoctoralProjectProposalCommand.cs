using DoctoralManagement.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace DoctoralManagement.Application.DoctoralProjects.Commands
{
    public class UploadDoctoralProjectProposalCommand : IRequest<UploadDoctoralProjectProposalResponse>
    {
        public int DoctoralProjectId { get; set; }
        public IFormFile File { get; set; } = null!;
        public string FileName { get; set; } = string.Empty;
        public ActivityDocumentType DocumentType { get; set; }
    }

    public class UploadDoctoralProjectProposalResponse
    {
        public string FileName { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public bool Success { get; set; }
    }
}
