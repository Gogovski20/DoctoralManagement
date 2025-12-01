using DoctoralManagement.Domain.Entities;
using DoctoralManagement.Domain.Interfaces;
using MediatR;

namespace DoctoralManagement.Application.Publications.Commands
{
    public class UploadPublicationDocumentHandler : IRequestHandler<UploadPublicationDocumentCommand, UploadPublicationDocumentResponse>
    {
        private readonly IPublicationRepository _publicationRepository;
        private readonly IFileService _fileService;
        private readonly IActivityDocumentRepository _publicationDocumentRepository;
        private readonly IStudentRepository _studentRepository;

        public UploadPublicationDocumentHandler(IPublicationRepository publicationRepository, IFileService fileService, IActivityDocumentRepository publicationDocumentRepository, IStudentRepository studentRepository)
        {
            _publicationRepository = publicationRepository;
            _fileService = fileService;
            _publicationDocumentRepository = publicationDocumentRepository;
            _studentRepository = studentRepository;
        }

        public async Task<UploadPublicationDocumentResponse> Handle(UploadPublicationDocumentCommand request, CancellationToken cancellationToken)
        {
            var publication = await _publicationRepository.GetByIdAsync(request.PublicationId)
                ?? throw new Exception("Publication not found.");

            var student = await _studentRepository.GetByIdAsync(publication.StudentId)
                ?? throw new Exception("Student not found.");

            if (publication.IsApproved)
            {
                throw new Exception("Cannot upload document for an approved publication.");
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
                throw new Exception("Invalid document type for publication.");
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
