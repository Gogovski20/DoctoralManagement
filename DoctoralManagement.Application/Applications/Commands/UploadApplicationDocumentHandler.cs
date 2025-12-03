using DoctoralManagement.Application.Common;
using DoctoralManagement.Domain.Entities;
using DoctoralManagement.Domain.Exceptions;
using DoctoralManagement.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Net;

namespace DoctoralManagement.Application.Applications.Commands
{
    public class UploadApplicationDocumentHandler : IRequestHandler<UploadApplicationDocumentCommand, UploadApplicationDocumentResponse>
    {
        private readonly IApplicationRepository _applicationRepository;
        private readonly IFileService _fileService;
        private IApplicationDocumentRepository _applicationDocumentRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IAuthService _authService;
        private readonly ILogger<UploadApplicationDocumentHandler> _logger;

        public UploadApplicationDocumentHandler(IApplicationRepository applicationRepository, IFileService fileService, IApplicationDocumentRepository applicationDocumentRepository, ICurrentUserService currentUserService, IAuthService authService, ILogger<UploadApplicationDocumentHandler> logger)
        {
            _applicationRepository = applicationRepository;
            _fileService = fileService;
            _applicationDocumentRepository = applicationDocumentRepository;
            _currentUserService = currentUserService;
            _authService = authService;
            _logger = logger;
        }

        public async Task<UploadApplicationDocumentResponse> Handle(UploadApplicationDocumentCommand request, CancellationToken cancellationToken)
        {
            var application = await _applicationRepository.GetByIdAsync(request.ApplicationId)
                ?? throw new DoctoralManagementException("Application not found.", HttpStatusCode.NotFound);

            if (application.ApplicationStatus != ApplicationStatus.Draft)
            {
                throw new Exception("Documents can only be uploaded for applications in Draft status.");
            }

            var currentUserId = _currentUserService.UserId;
            var linkedStudentId = await _authService.GetLinkedStudentIdAsync(currentUserId);

            if (linkedStudentId == null || linkedStudentId != application.StudentId)
            {
                throw new DoctoralManagementException(
                    "You can only upload documents for your own application.",
                    HttpStatusCode.Forbidden);
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
