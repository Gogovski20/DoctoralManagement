using DoctoralManagement.Application.Common;
using DoctoralManagement.Domain.Entities;
using DoctoralManagement.Domain.Exceptions;
using DoctoralManagement.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Net;

namespace DoctoralManagement.Application.ThesisDefenses
{
    public class UploadThesisDocumentHandler : IRequestHandler<UploadThesisDocumentCommand, UploadThesisDocumentResponse>
    {
        private readonly IDoctoralProjectRepository _projectRepo;
        private readonly IActivityDocumentRepository _docRepo;
        private readonly IFileService _fileService;
        private readonly ICurrentUserService _currentUserService;
        private readonly IAuthService _authService;
        private readonly ILogger<UploadThesisDocumentHandler> _logger;

        public UploadThesisDocumentHandler(IDoctoralProjectRepository projectRepo, IActivityDocumentRepository docRepo, IFileService fileService, ICurrentUserService currentUserService, IAuthService authService, ILogger<UploadThesisDocumentHandler> logger)
        {
            _projectRepo = projectRepo;
            _docRepo = docRepo;
            _fileService = fileService;
            _currentUserService = currentUserService;
            _authService = authService;
            _logger = logger;
        }

        public async Task<UploadThesisDocumentResponse> Handle(UploadThesisDocumentCommand request, CancellationToken cancellationToken)
        {
            var project = await _projectRepo.GetByIdAsync(request.ProjectId)
            ?? throw new DoctoralManagementException("Project not found", HttpStatusCode.NotFound);

            var currentUserId = _currentUserService.UserId;
            var linkedStudentId = await _authService.GetLinkedStudentIdAsync(currentUserId);

            if (linkedStudentId == null || linkedStudentId != project.StudentId)
            {
                throw new DoctoralManagementException(
                    "You can only upload documents for your own projects.",
                    HttpStatusCode.Forbidden);
            }

            if (project.Status != ProjectStatus.Completed && project.Status != ProjectStatus.DefenseChangesRequired)
                throw new DoctoralManagementException("Thesis upload allowed only after project completion.", HttpStatusCode.BadRequest);

            var uploadedFileName = _fileService.UploadFile(request.File, request.DocumentType.ToString());
            var newDoc = new ActivityDocument
            {
                DoctoralProjectId = project.Id,
                DocumentType = request.DocumentType,
                FileName = uploadedFileName,
                FilePath = _fileService.GetFilePath(request.File),
                FileSize = _fileService.GetFileSize(request.File).ToString(),
                ContentType = request.File.ContentType,
                UploadedAt = DateTime.UtcNow,
                Status = DocumentStatus.Pending
            };

            await _docRepo.AddAsync(newDoc);

            _logger.LogInformation(
                "Thesis document {FileName} uploaded for project {ProjectId} by student {StudentId}",
                uploadedFileName, request.ProjectId, linkedStudentId);

            return new UploadThesisDocumentResponse
            {
                Success = true,
                FileName = uploadedFileName,
                Message = "Thesis document uploaded for review."
            };
        }
    }
}
