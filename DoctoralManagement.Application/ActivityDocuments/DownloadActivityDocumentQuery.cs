using MediatR;

namespace DoctoralManagement.Application.ActivityDocuments
{
    public class DownloadActivityDocumentQuery : IRequest<DownloadDocumentResponse>
    {
        public int ActivityId { get; set; }
        public int DocumentId { get; set; }
        public ActivityType ActivityType { get; set; }
    }

    //public class DownloadDocumentResponse
    //{
    //    public byte[] FileBytes { get; set; }
    //    public string FileName { get; set; } = string.Empty;
    //    public string ContentType { get; set; } = string.Empty;
    //}

    public class DownloadDocumentResponse
    {
        public bool Success { get; set; } = true;
        public string Message { get; set; } = string.Empty;
        public byte[] FileBytes { get; set; } = Array.Empty<byte>();
        public string FileName { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
    }
}
