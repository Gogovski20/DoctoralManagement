using DoctoralManagement.Domain.Entities;
using DoctoralManagement.Domain.Interfaces;
using MediatR;

namespace DoctoralManagement.Application.Applications.Commands
{
    public class UploadApplicationDocumentHandler : IRequestHandler<UploadApplicationDocumentCommand, UploadApplicationDocumentResponse>
    {
        private readonly IApplicationRepository _applicationRepository;
        private readonly IFileService _fileService;
        private IApplicationDocumentRepository _applicationDocumentRepository;

        public UploadApplicationDocumentHandler(IApplicationRepository applicationRepository, IFileService fileService, IApplicationDocumentRepository applicationDocumentRepository)
        {
            _applicationRepository = applicationRepository;
            _fileService = fileService;
            _applicationDocumentRepository = applicationDocumentRepository;
        }

        public async Task<UploadApplicationDocumentResponse> Handle(UploadApplicationDocumentCommand request, CancellationToken cancellationToken)
        {
            var application = await _applicationRepository.GetByIdAsync(request.ApplicationId)
                ?? throw new Exception("Application not found.");

            if (application.ApplicationStatus != ApplicationStatus.Draft)
            {
                throw new Exception("Documents can only be uploaded for applications in Draft status.");
            }

            var existingDocument = await _applicationDocumentRepository.GetByApplicationAndTypeAsync(request.ApplicationId, request.Type);
            if (existingDocument != null)
            {
                return new UploadApplicationDocumentResponse
                {
                    FileName = string.Empty,
                    Message = "A document of this type already exists for the application.",
                    Success = false
                };
            }

            string cleanFileName = Path.GetFileNameWithoutExtension(request.FileName);
            var uploadedFileName = _fileService.UploadFile(request.File, cleanFileName);
            var uploadedFilePath = _fileService.GetFilePath(request.File);
            var uploadedFileSize = _fileService.GetFileSize(request.File);

            if (uploadedFileName.StartsWith("Invalid") || uploadedFileName.StartsWith("File size"))
            {
                return new UploadApplicationDocumentResponse { FileName = uploadedFileName, Success = false };
            }

            var newDocument = new ApplicationDocument
            {
                ApplicationId = request.ApplicationId,
                DocumentType = request.Type,
                FileName = uploadedFileName,
                FilePath = uploadedFileName,
                FileSize = uploadedFileSize.ToString(),
                ContentType = request.File.ContentType,
                UploadedAt = DateTime.UtcNow,
                UploadedBy = application.StudentId
            };

            if (application.Documents == null)
            {
                application.Documents = new List<ApplicationDocument>();
            }

            application.Documents.Add(newDocument);

            await _applicationRepository.UpdateAsync(application);

            return new UploadApplicationDocumentResponse
            {
                FileName = uploadedFileName,
                Message = "File uploaded successfully.",
                Success = true
            };
        }
    }
}
