using DoctoralManagement.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace DoctoralManagement.Application.ConferenceParticipations.Commands
{
    public class UploadConferenceDocumentCommand : IRequest<UploadConferenceDocumentResponse>
    {
        public int ConferenceId { get; set; }
        public IFormFile File { get; set; } = null!;
        public string FileName { get; set; } = string.Empty;
        public ActivityDocumentType Type { get; set; }
    }

    public class UploadConferenceDocumentResponse
    {
        public string FileName { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public bool Success { get; set; }
    }
}
