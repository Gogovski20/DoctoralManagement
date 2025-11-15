using DoctoralManagement.Domain.Entities;
using DoctoralManagement.Domain.Interfaces;
using MediatR;

namespace DoctoralManagement.Application.DoctoralProjects.Commands
{
    public class SubmitDoctoralProjectHandler : IRequestHandler<SubmitDoctoralProjectCommand, SubmitDoctoralProjectResponse>
    {
        private readonly IDoctoralProjectRepository _doctoralProjectRepository;
        private readonly IStudentRepository _studentRepository;
        private readonly IApplicationRepository _applicationRepository;

        public SubmitDoctoralProjectHandler(IDoctoralProjectRepository doctoralProjectRepository, IStudentRepository studentRepository, IApplicationRepository applicationRepository)
        {
            _doctoralProjectRepository = doctoralProjectRepository;
            _studentRepository = studentRepository;
            _applicationRepository = applicationRepository;
        }

        public async Task<SubmitDoctoralProjectResponse> Handle(SubmitDoctoralProjectCommand request, CancellationToken cancellationToken)
        {
            var project = await _doctoralProjectRepository.GetByIdAsync(request.ProjectId)
                ?? throw new Exception($"Doctoral project with id {request.ProjectId} not found");

            if (project.Status != ProjectStatus.Draft && project.Status != ProjectStatus.ChangesRequested)
            {
                throw new Exception("Only Draft or ChangesRequested projects can be sumbitted");
            }

            var student = await _studentRepository.GetByIdAsync(project.StudentId)
                ?? throw new Exception("Student not found");

            var hasAccepted = await _applicationRepository.HasFinalAcceptedApplicationAsync(project.StudentId);
            if (!hasAccepted)
            {
                throw new Exception("Student is not admitted to a doctoral program");
            }

            project.Status = ProjectStatus.Submitted;
            project.SubmittedAt = DateTime.UtcNow;

            await _doctoralProjectRepository.UpdateAsync(project);

            return new SubmitDoctoralProjectResponse
            {
                Id = project.Id,
                Title = project.Title,
                ResearchArea = project.ResearchArea,
                Status = project.Status.ToString(),
                SubmittedAt = project.SubmittedAt.Value
            };
        }
    }
}
