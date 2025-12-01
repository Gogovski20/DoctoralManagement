using DoctoralManagement.Domain.Entities;
using Microsoft.AspNetCore.Http;

namespace DoctoralManagement.Application.Dtos
{
    public class UploadActivityDocumentDto
    {
        public IFormFile File { get; set; }
        public string FileName { get; set; } = string.Empty;
        public ActivityDocumentType Type { get; set; }
    }
}
