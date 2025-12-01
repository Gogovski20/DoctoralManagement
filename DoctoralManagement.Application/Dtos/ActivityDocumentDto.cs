using DoctoralManagement.Domain.Entities;

namespace DoctoralManagement.Application.Dtos
{
    public class ActivityDocumentDto
    {
        public int Id { get; set; }
        public ActivityDocumentType Type { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
        public DateTime UploadedAt { get; set; }
        public string ReviewComment { get; set; } = string.Empty;
        public int ReviewedBy { get; set; }
        public DateTime ReviewedAt { get; set; } 
    }
}
