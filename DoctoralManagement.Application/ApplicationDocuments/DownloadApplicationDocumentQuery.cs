using MediatR;

namespace DoctoralManagement.Application.ApplicationDocuments
{
    public class DownloadApplicationDocumentQuery : IRequest<DownloadDocumentResponse>
    {
        public int ApplicationId { get; set; }
        public int DocumentId { get; set; }
    }

    public class DownloadDocumentResponse
    {
        public byte[] FileBytes { get; set; } 
        public string FileName { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
    }
}
