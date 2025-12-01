using DoctoralManagement.Application.ECTS.Services;
using DoctoralManagement.Domain.Entities;
using DoctoralManagement.Domain.Interfaces;
using MediatR;

namespace DoctoralManagement.Application.ThesisDefenses
{
    public class ReviewThesisDocumentHandler : IRequestHandler<ReviewThesisDocumentCommand, ReviewThesisDocumentResponse>
    {
        private readonly IActivityDocumentRepository _docRepo;
        private readonly IDoctoralProjectRepository _projectRepo;
        private readonly IEctsTrackingRepository _ectsRepo;
        private readonly EctsProgressService _ectsProgressService;

        public ReviewThesisDocumentHandler(IActivityDocumentRepository docRepo, IDoctoralProjectRepository projectRepo, IEctsTrackingRepository ectsRepo, EctsProgressService ectsProgressService)
        {
            _docRepo = docRepo;
            _projectRepo = projectRepo;
            _ectsRepo = ectsRepo;
            _ectsProgressService = ectsProgressService;
        }

        public async Task<ReviewThesisDocumentResponse> Handle(ReviewThesisDocumentCommand request, CancellationToken cancellationToken)
        {
            var document = await _docRepo.GetByIdAsync(request.DocumentId)
                ?? throw new Exception("Document not found");

            if (document.DocumentType != ActivityDocumentType.DefenseThesisDocument)
            {
                throw new Exception("Invalid document type for thesis defense review");
            }

            var project = await _projectRepo.GetByIdAsync(document.DoctoralProjectId!.Value)
                ?? throw new Exception("Associated doctoral project not found");

            document.Status = request.NewStatus;
            document.ReviewComment = request.ReviewComment;
            document.ReviewedAt = DateTime.UtcNow;

            int ectsAwarded = 0;
            if (request.NewStatus == DocumentStatus.Approved)
            {
                project.Status = ProjectStatus.DefenseUnderReview;
                var ects = await _ectsRepo.GetByStudentIdAsync(project.StudentId);
                if (ects != null)
                {
                    ects.ThesisDefence += 20;
                    if (ects.ThesisDefence > 46) ects.ThesisDefence = 46;
                    ectsAwarded = 20;

                    await _ectsRepo.UpdateAsync(ects);
                    await _ectsProgressService.UpdateStudentSemesterAsync(project.StudentId, ects.TotalECTS);
                }
            }
            else if (request.NewStatus == DocumentStatus.Rejected)
            {
                project.Status = ProjectStatus.DefenseChangesRequired;
            }

            await _projectRepo.UpdateAsync(project);
            await _docRepo.UpdateAsync(document);

            return new ReviewThesisDocumentResponse
            {
                DocumentId = request.DocumentId,
                DocumentStatus = document.Status.ToString(),
                ProjectStatus = project.Status.ToString(),
                ReviewComment = document.ReviewComment,
                UpdatedECTS = ectsAwarded
            };
        }
    }
}
