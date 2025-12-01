using DoctoralManagement.Domain.Interfaces;
using MediatR;

namespace DoctoralManagement.Application.ApplicationDocuments
{
    public class DownloadApplicationDocumentHandler : IRequestHandler<DownloadApplicationDocumentQuery, DownloadDocumentResponse>
    {
        private readonly IApplicationDocumentRepository _documentRepository;
        private readonly IFileService _fileService;

        public DownloadApplicationDocumentHandler(IApplicationDocumentRepository documentRepository, IFileService fileService)
        {
            _documentRepository = documentRepository;
            _fileService = fileService;
        }

        public async Task<DownloadDocumentResponse> Handle(DownloadApplicationDocumentQuery request, CancellationToken cancellationToken)
        {
            var document = await _documentRepository.GetByIdAsync(request.DocumentId)
                ?? throw new Exception("Document not found.");

            var fileBytes = _fileService.ReadFile(document.FilePath);

            return new DownloadDocumentResponse
            {
                FileBytes = fileBytes,
                FileName = document.FileName,
                ContentType = document.ContentType
            };
        }
    }
}
