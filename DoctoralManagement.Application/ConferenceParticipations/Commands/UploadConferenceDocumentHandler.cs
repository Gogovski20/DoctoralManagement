using DoctoralManagement.Domain.Entities;
using DoctoralManagement.Domain.Interfaces;
using MediatR;

namespace DoctoralManagement.Application.ConferenceParticipations.Commands
{
    public class UploadConferenceDocumentHandler : IRequestHandler<UploadConferenceDocumentCommand, UploadConferenceDocumentResponse>
    {
        private readonly IConferenceParticipationRepository _conferenceRepository;
        private readonly IFileService _fileService;
        private readonly IActivityDocumentRepository _activityDocumentRepository;
        private readonly IStudentRepository _studentRepository;

        public UploadConferenceDocumentHandler(IConferenceParticipationRepository conferenceRepository, IFileService fileService, IActivityDocumentRepository activityDocumentRepository, IStudentRepository studentRepository)
        {
            _conferenceRepository = conferenceRepository;
            _fileService = fileService;
            _activityDocumentRepository = activityDocumentRepository;
            _studentRepository = studentRepository;
        }

        public async Task<UploadConferenceDocumentResponse> Handle(UploadConferenceDocumentCommand request, CancellationToken cancellationToken)
        {
            var conference = await _conferenceRepository.GetByIdAsync(request.ConferenceId)
                ?? throw new Exception("Conference participation not found.");

            var student = await _studentRepository.GetByIdAsync(conference.StudentId)
                ?? throw new Exception("Student not found.");

            if (conference.IsApproved == true)
            {
                throw new Exception("Cannot upload document for an approved conference participation.");
            }

            var existingDocument = await _activityDocumentRepository.GetByConferenceIdAsync(request.ConferenceId);
            if (existingDocument != null)
            {
                return new UploadConferenceDocumentResponse
                {
                    Success = false,
                    Message = "A document has already been uploaded for this conference participation.",
                    FileName = existingDocument.FileName
                };
            }

            if (request.Type != ActivityDocumentType.ConferenceProof)
            {
                throw new Exception("Invalid document type for conference participation.");
            }

            string cleanFileName = Path.GetFileNameWithoutExtension(request.FileName);
            var uploadedFileName = _fileService.UploadFile(request.File, cleanFileName);
            var uploadedFilePath = _fileService.GetFilePath(request.File);
            var uploadedFileSize = _fileService.GetFileSize(request.File);

            if (uploadedFileName.StartsWith("Invalid") || uploadedFileName.StartsWith("File size"))
            {
                return new UploadConferenceDocumentResponse { FileName = uploadedFileName, Success = false };
            }

            var newDocument = new ActivityDocument
            {
                ConferenceId = request.ConferenceId,
                DocumentType = request.Type,
                FileName = uploadedFileName,
                FilePath = uploadedFileName,
                FileSize = uploadedFileSize.ToString(),
                ContentType = request.File.ContentType,
                Status = DocumentStatus.Pending,
                UploadedBy = student.Id,
                UploadedAt = DateTime.UtcNow
            };

            conference.Document = newDocument;
            await _conferenceRepository.UpdateAsync(conference);

            return new UploadConferenceDocumentResponse
            {
                Success = true,
                Message = "Document uploaded successfully.",
                FileName = uploadedFileName
            };
        }
    }
}
