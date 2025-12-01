using DoctoralManagement.Domain.Entities;
using DoctoralManagement.Domain.Interfaces;
using MediatR;

namespace DoctoralManagement.Application.DoctoralProjects.Commands
{
    public class UploadDoctoralProjectProposalHandler : IRequestHandler<UploadDoctoralProjectProposalCommand, UploadDoctoralProjectProposalResponse>
    {
        private readonly IDoctoralProjectRepository _doctoralProjectRepository;
        private readonly IStudentRepository _studentRepository;
        private readonly IFileService _fileService;
        private readonly IActivityDocumentRepository _activityDocumentRepository;

        public UploadDoctoralProjectProposalHandler(IDoctoralProjectRepository doctoralProjectRepository, IStudentRepository studentRepository, IFileService fileService, IActivityDocumentRepository activityDocumentRepository)
        {
            _doctoralProjectRepository = doctoralProjectRepository;
            _studentRepository = studentRepository;
            _fileService = fileService;
            _activityDocumentRepository = activityDocumentRepository;
        }

        public async Task<UploadDoctoralProjectProposalResponse> Handle(UploadDoctoralProjectProposalCommand request, CancellationToken cancellationToken)
        {
            var project = await _doctoralProjectRepository.GetByIdAsync(request.DoctoralProjectId)
                ?? throw new Exception("Doctoral project not found.");

            var student = await _studentRepository.GetByIdAsync(project.StudentId)
                ?? throw new Exception("Student not found.");

            if (project.Status != ProjectStatus.Draft && project.Status != ProjectStatus.ChangesRequested)
            {
                throw new Exception("Document upload is not allowed in the current project status.");
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
                throw new Exception("Invalid document type for doctoral project proposal.");
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

            return new UploadDoctoralProjectProposalResponse
            {
                Success = true,
                Message = "Document uploaded successfully.",
                FileName = uploadedFileName
            };
        }
    }
}
