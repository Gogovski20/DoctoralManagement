namespace DoctoralManagement.Application.Applications.Commands
{
    public class UploadApplicationDocumentResponse
    {
        public string FileName { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public bool Success { get; set; }
    }
}
