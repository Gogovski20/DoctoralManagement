using DoctoralManagement.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace DoctoralManagement.Application.Publications.Commands
{
    public class UploadPublicationDocumentCommand : IRequest<UploadPublicationDocumentResponse>
    {
        public int PublicationId { get; set; }
        public IFormFile File { get; set; } = null!;
        public string FileName { get; set; } = string.Empty;
        public ActivityDocumentType Type { get; set; }
    }

    public class UploadPublicationDocumentResponse
    {
        public string FileName { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public bool Success { get; set; }
    }
}
