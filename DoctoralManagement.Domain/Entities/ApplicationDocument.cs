namespace DoctoralManagement.Domain.Entities
{
    public class ApplicationDocument
    {
        public int Id { get; set; }
        public int ApplicationId { get; set; }
        public ApplicationDocumentType DocumentType { get; set; }

        public string FileName { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public string FileSize { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
        public int UploadedBy { get; set; }
        public DateTime UploadedAt { get; set; }


        // Navigation 
        public Application Application { get; set; } = null!;
    }

    public enum ApplicationDocumentType
    {
        MotivationLetter = 1,
        ResearchProposal = 2,
        CV = 3,
        BachelorDiploma = 4,
        MasterDiploma = 5,
        Transcript = 6,
        EnglishCertificate = 7,
        Publication = 8,
        InitialMentorConsent = 9
    }
}
