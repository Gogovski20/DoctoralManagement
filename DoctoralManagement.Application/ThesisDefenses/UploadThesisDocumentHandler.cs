using DoctoralManagement.Domain.Entities;
using DoctoralManagement.Domain.Interfaces;
using MediatR;

namespace DoctoralManagement.Application.ThesisDefenses
{
    public class UploadThesisDocumentHandler : IRequestHandler<UploadThesisDocumentCommand, UploadThesisDocumentResponse>
    {
        private readonly IDoctoralProjectRepository _projectRepo;
        private readonly IActivityDocumentRepository _docRepo;
        private readonly IFileService _fileService;

        public UploadThesisDocumentHandler(IDoctoralProjectRepository projectRepo, IActivityDocumentRepository docRepo, IFileService fileService)
        {
            _projectRepo = projectRepo;
            _docRepo = docRepo;
            _fileService = fileService;
        }

        public async Task<UploadThesisDocumentResponse> Handle(UploadThesisDocumentCommand request, CancellationToken cancellationToken)
        {
            var project = await _projectRepo.GetByIdAsync(request.ProjectId)
            ?? throw new Exception("Project not found");

            if (project.Status != ProjectStatus.Completed && project.Status != ProjectStatus.DefenseChangesRequired)
                throw new Exception("Thesis upload allowed only after project completion.");

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

            return new UploadThesisDocumentResponse
            {
                Success = true,
                FileName = uploadedFileName,
                Message = "Thesis document uploaded for review."
            };
        }
    }
}
