using DoctoralManagement.Application.Common;
using DoctoralManagement.Domain.Entities;
using DoctoralManagement.Domain.Exceptions;
using DoctoralManagement.Domain.Interfaces;
using MediatR;
using System.Net;

namespace DoctoralManagement.Application.Publications.Commands
{
    public class UploadPublicationDocumentHandler : IRequestHandler<UploadPublicationDocumentCommand, UploadPublicationDocumentResponse>
    {
        private readonly IPublicationRepository _publicationRepository;
        private readonly IFileService _fileService;
        private readonly IActivityDocumentRepository _publicationDocumentRepository;
        private readonly IStudentRepository _studentRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IAuthService _authService;

        public UploadPublicationDocumentHandler(IPublicationRepository publicationRepository, IFileService fileService, IActivityDocumentRepository publicationDocumentRepository, IStudentRepository studentRepository, ICurrentUserService currentUserService, IAuthService authService)
        {
            _publicationRepository = publicationRepository;
            _fileService = fileService;
            _publicationDocumentRepository = publicationDocumentRepository;
            _studentRepository = studentRepository;
            _currentUserService = currentUserService;
            _authService = authService;
        }

        public async Task<UploadPublicationDocumentResponse> Handle(UploadPublicationDocumentCommand request, CancellationToken cancellationToken)
        {
            var publication = await _publicationRepository.GetByIdAsync(request.PublicationId)
                ?? throw new DoctoralManagementException("Publication not found.", HttpStatusCode.NotFound);

            var currentUserId = _currentUserService.UserId;
            var linkedStudentId = await _authService.GetLinkedStudentIdAsync(currentUserId);

            if (linkedStudentId == null || linkedStudentId != publication.StudentId)
            {
                throw new DoctoralManagementException(
                    "You can only upload documents for your own conference participations.",
                    HttpStatusCode.Forbidden);
            }

            var student = await _studentRepository.GetByIdAsync(publication.StudentId)
                ?? throw new DoctoralManagementException("Student not found.", HttpStatusCode.NotFound);

            if (publication.IsApproved)
            {
                throw new DoctoralManagementException("Cannot upload document for an approved publication.", HttpStatusCode.BadRequest);
            }

            var existingDocument = await _publicationDocumentRepository.GetByPublicationIdAsync(request.PublicationId);
            if (existingDocument != null)
            {
                return new UploadPublicationDocumentResponse
                {
                    Success = false,
                    Message = "A document has already been uploaded for this publication.",
                    FileName = existingDocument.FileName
                };
            }

            if (request.Type != ActivityDocumentType.PublicationProof)
            {
                throw new DoctoralManagementException("Invalid document type for publication.", HttpStatusCode.BadRequest);
            }

            string cleanFileName = Path.GetFileNameWithoutExtension(request.FileName);
            var uploadedFileName = _fileService.UploadFile(request.File, cleanFileName);
            var uploadedFilePath = _fileService.GetFilePath(request.File);
            var uploadedFileSize = _fileService.GetFileSize(request.File);

            if (uploadedFileName.StartsWith("Invalid") || uploadedFileName.StartsWith("File size"))
            {
                return new UploadPublicationDocumentResponse { FileName = uploadedFileName, Success = false };
            }

            var newDocument = new ActivityDocument
            {
                PublicationId = request.PublicationId,
                DocumentType = request.Type,
                FileName = uploadedFileName,
                FilePath = uploadedFileName,
                FileSize = uploadedFileSize.ToString(),
                ContentType = request.File.ContentType,
                Status = DocumentStatus.Pending,
                UploadedBy = student.Id,
                UploadedAt = DateTime.UtcNow
            };

            publication.Document = newDocument;
            await _publicationRepository.UpdateAsync(publication);

            return new UploadPublicationDocumentResponse
            {
                Success = true,
                Message = "Document uploaded successfully.",
                FileName = uploadedFileName
            };
        }
    }
}
