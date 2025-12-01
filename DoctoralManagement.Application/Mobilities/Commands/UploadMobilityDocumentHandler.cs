using DoctoralManagement.Application.Publications.Commands;
using DoctoralManagement.Domain.Entities;
using DoctoralManagement.Domain.Interfaces;
using MediatR;

namespace DoctoralManagement.Application.Mobilities.Commands
{
    public class UploadMobilityDocumentHandler : IRequestHandler<UploadMobilityDocumentCommand, UploadMobilityDocumentResponse>
    {
        private readonly IMobilityRepository _mobilityRepository;
        private readonly IFileService _fileService;
        private readonly IActivityDocumentRepository _activityDocumentRepository;
        private readonly IStudentRepository _studentRepository;

        public UploadMobilityDocumentHandler(IMobilityRepository mobilityRepository, IFileService fileService, IActivityDocumentRepository activityDocumentRepository, IStudentRepository studentRepository)
        {
            _mobilityRepository = mobilityRepository;
            _fileService = fileService;
            _activityDocumentRepository = activityDocumentRepository;
            _studentRepository = studentRepository;
        }

        public async Task<UploadMobilityDocumentResponse> Handle(UploadMobilityDocumentCommand request, CancellationToken cancellationToken)
        {
            var mobility = await _mobilityRepository.GetByIdAsync(request.MobilityId)
                ?? throw new Exception("Mobility not found");

            var student = await _studentRepository.GetByIdAsync(mobility.StudentId)
                ?? throw new Exception("Student not found");

            if (mobility.IsApproved)
            {
                throw new Exception("Cannot upload document to an approved mobility");
            }

            var existingDocument = await _activityDocumentRepository.GetByMobilityIdAsync(request.MobilityId);
            if (existingDocument != null)
            {
                return new UploadMobilityDocumentResponse
                {
                    Success = false,
                    Message = "A document has already been uploaded for this mobility.",
                    FileName = existingDocument.FileName
                };
            }

            if (request.Type != ActivityDocumentType.MobilityProof)
            {
                throw new Exception("Invalid document type for mobility");
            }
            string cleanFileName = Path.GetFileNameWithoutExtension(request.FileName);
            var uploadedFileName = _fileService.UploadFile(request.File, cleanFileName);
            var uploadedFilePath = _fileService.GetFilePath(request.File);
            var uploadedFileSize = _fileService.GetFileSize(request.File);

            if (uploadedFileName.StartsWith("Invalid") || uploadedFileName.StartsWith("File size"))
            {
                return new UploadMobilityDocumentResponse { FileName = uploadedFileName, Success = false };
            }

            var newDocument = new ActivityDocument
            {
                MobilityId = request.MobilityId,
                DocumentType = request.Type,
                FileName = uploadedFileName,
                FilePath = uploadedFileName,
                FileSize = uploadedFileSize.ToString(),
                ContentType = request.File.ContentType,
                Status = DocumentStatus.Pending,
                UploadedBy = student.Id,
                UploadedAt = DateTime.UtcNow
            };

            await _activityDocumentRepository.AddAsync(newDocument);

            mobility.Document = newDocument;
            await _mobilityRepository.UpdateAsync(mobility);

            return new UploadMobilityDocumentResponse
            {
                Success = true,
                Message = "Document uploaded successfully",
                FileName = uploadedFileName
            };
        }
    }
}
