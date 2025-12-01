using DoctoralManagement.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace DoctoralManagement.Application.Applications.Commands
{
    public class UploadApplicationDocumentCommand : IRequest<UploadApplicationDocumentResponse>
    {
        public int ApplicationId { get; set; }
        public IFormFile File { get; set; }
        public string FileName { get; set; } = string.Empty;
        public ApplicationDocumentType Type { get; set; }
    }
}
