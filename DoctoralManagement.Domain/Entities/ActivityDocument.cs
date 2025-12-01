namespace DoctoralManagement.Domain.Entities
{
    public class ActivityDocument
    {
        public int Id { get; set; }
        public ActivityDocumentType DocumentType { get; set; }

        public string FileName { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public string FileSize { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;

        public DocumentStatus Status { get; set; } = DocumentStatus.Pending;
        public string? ReviewComment { get; set; }
        public int? ReviewedBy { get; set; }
        public DateTime? ReviewedAt { get; set; }

        public int UploadedBy { get; set; }
        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

        public int? PublicationId { get; set; }
        public Publication? Publication { get; set; }

        public int? MobilityId { get; set; }
        public Mobility? Mobility { get; set; }

        public int? ConferenceId { get; set; }
        public ConferenceParticipation? Conference { get; set; }

        public int? DoctoralProjectId { get; set; } 
        public DoctoralProject? DoctoralProject { get; set; }
    }

    public enum ActivityDocumentType
    {
        PublicationProof = 1,
        MobilityProof = 2,
        ConferenceProof = 3,
        DoctoralProjectReport = 4,
        DefenseThesisDocument = 5
    }

    public enum DocumentStatus
    {
        Pending = 1,
        Approved = 2,
        Rejected = 3
    }
}
