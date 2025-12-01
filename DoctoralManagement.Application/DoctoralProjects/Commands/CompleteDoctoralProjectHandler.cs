using DoctoralManagement.Application.ECTS.Services;
using DoctoralManagement.Domain.Interfaces;
using MediatR;

namespace DoctoralManagement.Application.DoctoralProjects.Commands
{
    public class CompleteDoctoralProjectHandler : IRequestHandler<CompleteDoctoralProjectCommand, CompleteDoctoralProjectResponse>
    {
        private readonly IDoctoralProjectRepository _projectRepo;
        private readonly IEctsTrackingRepository _ectsRepo;
        private readonly EctsProgressService _ectsProgressService;

        public CompleteDoctoralProjectHandler(IDoctoralProjectRepository projectRepo, IEctsTrackingRepository ectsRepo, EctsProgressService ectsProgressService)
        {
            _projectRepo = projectRepo;
            _ectsRepo = ectsRepo;
            _ectsProgressService = ectsProgressService;
        }

        public async Task<CompleteDoctoralProjectResponse> Handle(CompleteDoctoralProjectCommand request, CancellationToken cancellationToken)
        {
            var project = await _projectRepo.GetByIdAsync(request.ProjectId)
                ?? throw new Exception("Doctoral project not found.");

            if (project.Status != Domain.Entities.ProjectStatus.Approved && project.Status != Domain.Entities.ProjectStatus.UnderReview)
            {
                throw new Exception("Only approved or under review projects can be completed.");
            }

            project.Status = Domain.Entities.ProjectStatus.Completed;
            project.DecisionAt = DateTime.UtcNow;
            await _projectRepo.UpdateAsync(project);

            var ects = await _ectsRepo.GetByStudentIdAsync(project.StudentId);
            if (ects != null)
            {
                ects.IndependentResearchProject += 27;
                if (ects.IndependentResearchProject > 27)
                {
                    ects.IndependentResearchProject = 27;
                }
                await _ectsRepo.UpdateAsync(ects);
                await _ectsProgressService.UpdateStudentSemesterAsync(project.StudentId, ects.TotalECTS);
            }

            return new CompleteDoctoralProjectResponse
            {
                ProjectId = project.Id,
                Status = project.Status.ToString(),
                CompletedAt = project.DecisionAt.Value
            };
        }
    }
}
