using DoctoralManagement.Domain.Interfaces;
using MediatR;

namespace DoctoralManagement.Application.DoctoralProjects.Commands
{
    public class UpdateDoctoralProjectHandler : IRequestHandler<UpdateDoctoralProjectCommand, UpdateDoctoralProjectResponse>
    {
        private readonly IDoctoralProjectRepository _doctoralProjectRepository;
        private readonly IMentorRepository _mentorRepository;

        public UpdateDoctoralProjectHandler(IDoctoralProjectRepository doctoralProjectRepository, IMentorRepository mentorRepository)
        {
            _doctoralProjectRepository = doctoralProjectRepository;
            _mentorRepository = mentorRepository;
        }

        public async Task<UpdateDoctoralProjectResponse> Handle(UpdateDoctoralProjectCommand request, CancellationToken cancellationToken)
        {
            var project = await _doctoralProjectRepository.GetByIdAsync(request.Id)
                ?? throw new Exception($"Doctoral project with id {request.Id} not found.");

            if (project.Status != Domain.Entities.ProjectStatus.Draft && project.Status != Domain.Entities.ProjectStatus.ChangesRequested)
            {
                throw new Exception("Only projects in Draft or ChangesRequested status can be updated.");
            }

            var mentor = await _mentorRepository.GetByIdAsync(request.MentorId)
                ?? throw new Exception($"Mentor with id {request.MentorId} not found.");

            project.Title = request.Title;
            project.ResearchArea = request.ResearchArea;
            project.EctsCredits = request.EctsCredits;
            project.MentorId = request.MentorId;
            project.ProposalDocumentPath = request.ProposalDocumentPath ?? project.ProposalDocumentPath;

            await _doctoralProjectRepository.UpdateAsync(project);

            return new UpdateDoctoralProjectResponse
            {
                Id = project.Id,
                Title = project.Title,
                ResearchArea = project.ResearchArea,
                EctsCredits = project.EctsCredits,
                Status = project.Status.ToString(),
                MentorId = project.MentorId,
                ProposalDocumentPath = project.ProposalDocumentPath,
                CreatedAt = project.CreatedAt,
                SubmittedAt = project.SubmittedAt
            };
        }
    }
}
