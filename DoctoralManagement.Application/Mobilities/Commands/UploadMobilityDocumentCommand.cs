using DoctoralManagement.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace DoctoralManagement.Application.Mobilities.Commands
{
    public class UploadMobilityDocumentCommand : IRequest<UploadMobilityDocumentResponse>
    {
        public int MobilityId { get; set; }
        public IFormFile File { get; set; } = null!;
        public string FileName { get; set; } = string.Empty;
        public ActivityDocumentType Type { get; set; }
    }
    

    public class UploadMobilityDocumentResponse
    {
        public string FileName { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public bool Success { get; set; }
    }
}
