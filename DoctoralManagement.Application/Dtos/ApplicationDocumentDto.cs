using DoctoralManagement.Domain.Entities;

namespace DoctoralManagement.Application.Dtos
{
    public class ApplicationDocumentDto
    {
        public int Id { get; set; }
        public ApplicationDocumentType DocumentType { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
        public DateTime UploadedAt { get; set; }
    }
}
