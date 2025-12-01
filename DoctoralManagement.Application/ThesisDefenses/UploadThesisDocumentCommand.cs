using DoctoralManagement.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace DoctoralManagement.Application.ThesisDefenses
{
    public class UploadThesisDocumentCommand : IRequest<UploadThesisDocumentResponse>
    {
        public int ProjectId { get; set; }
        public IFormFile File { get; set; }
        public string FileName { get; set; } = string.Empty;
        public ActivityDocumentType DocumentType { get; set; } = ActivityDocumentType.DefenseThesisDocument;
    }

    public class UploadThesisDocumentResponse
    {
        public bool Success { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }
}
