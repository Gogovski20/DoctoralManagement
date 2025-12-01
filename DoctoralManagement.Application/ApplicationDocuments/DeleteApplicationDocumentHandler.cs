using DoctoralManagement.Domain.Interfaces;
using MediatR;

namespace DoctoralManagement.Application.ApplicationDocuments
{
    public class DeleteApplicationDocumentHandler : IRequestHandler<DeleteApplicationDocumentCommand, bool>
    {
        private readonly IApplicationRepository _applicationRepository;
        private readonly IApplicationDocumentRepository _documentRepository;
        private readonly IFileService _fileService;

        public DeleteApplicationDocumentHandler(IApplicationRepository applicationRepository, IApplicationDocumentRepository documentRepository, IFileService fileService)
        {
            _applicationRepository = applicationRepository;
            _documentRepository = documentRepository;
            _fileService = fileService;
        }

        public async Task<bool> Handle(DeleteApplicationDocumentCommand request, CancellationToken cancellationToken)
        {
            var application = await _applicationRepository.GetByIdAsync(request.ApplicationId)
                ?? throw new Exception("Application not found.");

            if (application.ApplicationStatus != Domain.Entities.ApplicationStatus.Draft)
            {
                throw new Exception("Cannot delete documents from an application that is not in Draft status.");
            }

            var document = await _documentRepository.GetByIdAsync(request.DocumentId)
                ?? throw new Exception("Document not found.");

            _fileService.DeleteFile(document.FilePath);
            await _documentRepository.DeleteAsync(document);
            return true;
        }
    }
}
