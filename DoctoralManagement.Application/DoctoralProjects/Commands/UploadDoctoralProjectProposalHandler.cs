using DoctoralManagement.Application.Common;
using DoctoralManagement.Domain.Entities;
using DoctoralManagement.Domain.Exceptions;
using DoctoralManagement.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Net;

namespace DoctoralManagement.Application.DoctoralProjects.Commands
{
    public class UploadDoctoralProjectProposalHandler : IRequestHandler<UploadDoctoralProjectProposalCommand, UploadDoctoralProjectProposalResponse>
    {
        private readonly IDoctoralProjectRepository _doctoralProjectRepository;
        private readonly IStudentRepository _studentRepository;
        private readonly IFileService _fileService;
        private readonly IActivityDocumentRepository _activityDocumentRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IAuthService _authService;
        private readonly ILogger<UploadDoctoralProjectProposalHandler> _logger;

        public UploadDoctoralProjectProposalHandler(IDoctoralProjectRepository doctoralProjectRepository, IStudentRepository studentRepository, IFileService fileService, IActivityDocumentRepository activityDocumentRepository, ICurrentUserService currentUserService, IAuthService authService, ILogger<UploadDoctoralProjectProposalHandler> logger)
        {
            _doctoralProjectRepository = doctoralProjectRepository;
            _studentRepository = studentRepository;
            _fileService = fileService;
            _activityDocumentRepository = activityDocumentRepository;
            _currentUserService = currentUserService;
            _authService = authService;
            _logger = logger;
        }

        public async Task<UploadDoctoralProjectProposalResponse> Handle(UploadDoctoralProjectProposalCommand request, CancellationToken cancellationToken)
        {
            var project = await _doctoralProjectRepository.GetByIdAsync(request.DoctoralProjectId)
                ?? throw new DoctoralManagementException("Doctoral project not found.", HttpStatusCode.NotFound);

            var currentUserId = _currentUserService.UserId;
            var linkedStudentId = await _authService.GetLinkedStudentIdAsync(currentUserId);

            if (linkedStudentId == null || linkedStudentId != project.StudentId)
            {
                throw new DoctoralManagementException(
                    "You can only upload documents for your own doctoral projects.",
                    HttpStatusCode.Forbidden);
            }

            var student = await _studentRepository.GetByIdAsync(project.StudentId)
                ?? throw new DoctoralManagementException("Student not found.", HttpStatusCode.NotFound);

            if (project.Status != ProjectStatus.Draft && project.Status != ProjectStatus.ChangesRequested)
            {
                throw new DoctoralManagementException("Document upload is not allowed in the current project status.", HttpStatusCode.BadRequest);
            }

            var documents = project.Documents;
            foreach (var doc in documents)
            {
                if (doc.DocumentType == request.DocumentType)
                {
                    return new UploadDoctoralProjectProposalResponse
                    {
                        Success = false,
                        Message = "A document has already been uploaded for this conference participation.",
                        FileName = doc.FileName
                    };
                }
            }

            if (request.DocumentType != ActivityDocumentType.DoctoralProjectReport && request.DocumentType != ActivityDocumentType.DefenseThesisDocument)
            {
                throw new DoctoralManagementException("Invalid document type for doctoral project proposal.", HttpStatusCode.BadRequest);
            }

            string cleanFileName = Path.GetFileNameWithoutExtension(request.FileName);
            var uploadedFileName = _fileService.UploadFile(request.File, cleanFileName);
            var uploadedFilePath = _fileService.GetFilePath(request.File);
            var uploadedFileSize = _fileService.GetFileSize(request.File);

            if (uploadedFileName.StartsWith("Invalid") || uploadedFileName.StartsWith("File size"))
            {
                return new UploadDoctoralProjectProposalResponse { FileName = uploadedFileName, Success = false };
            }

            var newDocument = new ActivityDocument
            {
                DoctoralProjectId = request.DoctoralProjectId,
                DocumentType = request.DocumentType,
                FileName = uploadedFileName,
                FilePath = uploadedFileName,
                FileSize = uploadedFileSize.ToString(),
                ContentType = request.File.ContentType,
                Status = DocumentStatus.Pending,
                UploadedBy = student.Id,
                UploadedAt = DateTime.UtcNow
            };

            if (project.Documents == null)
            {
                project.Documents = new List<ActivityDocument>();
            }

            project.Documents.Add(newDocument);
            await _doctoralProjectRepository.UpdateAsync(project);

            _logger.LogInformation(
                "Doctoral project document {FileName} uploaded for project {ProjectId} by student {StudentId}",
                uploadedFileName, request.DoctoralProjectId, linkedStudentId);

            return new UploadDoctoralProjectProposalResponse
            {
                Success = true,
                Message = "Document uploaded successfully.",
                FileName = uploadedFileName
            };
        }
    }
}
