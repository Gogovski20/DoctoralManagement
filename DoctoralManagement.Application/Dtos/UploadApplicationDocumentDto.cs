using DoctoralManagement.Domain.Entities;
using Microsoft.AspNetCore.Http;

namespace DoctoralManagement.Application.Dtos
{
    public class UploadApplicationDocumentDto
    {
        public IFormFile File { get; set; }
        public string FileName { get; set; } = string.Empty;
        public ApplicationDocumentType Type { get; set; }
    }
}
