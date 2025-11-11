using DoctoralManagement.Domain.Entities;
using DoctoralManagement.Domain.Interfaces;
using MediatR;

namespace DoctoralManagement.Application.DoctoralProjects.Commands
{
    public class SubmitDoctoralProjectHandler : IRequestHandler<SubmitDoctoralProjectCommand, SubmitDoctoralProjectResponse>
    {
        private readonly IDoctoralProjectRepository _doctoralProjectRepository;
        private readonly IStudentRepository _studentRepository;
        private readonly IMentorRepository _mentorRepository;

        public SubmitDoctoralProjectHandler(IDoctoralProjectRepository doctoralProjectRepository, IStudentRepository studentRepository, IMentorRepository mentorRepository)
        {
            _doctoralProjectRepository = doctoralProjectRepository;
            _studentRepository = studentRepository;
            _mentorRepository = mentorRepository;
        }

        public async Task<SubmitDoctoralProjectResponse> Handle(SubmitDoctoralProjectCommand request, CancellationToken cancellationToken)
        {
            var student = await _studentRepository.GetByIdAsync(request.StudentId)
                ?? throw new Exception($"Student with id {request.StudentId} not found");

            var mentor = await _mentorRepository.GetByIdAsync(request.MentorId)
                ?? throw new Exception($"Mentor with id {request.MentorId} not found");

            if (await _doctoralProjectRepository.ExistsActiveProjectForStudentAsync(request.StudentId))
            {
                throw new Exception("Student already has an active doctoral project");
            }

            var project = new DoctoralProject
            {
                Title = request.Title,
                ResearchArea = request.ResearchArea,
                StudentId = request.StudentId,
                MentorId = request.MentorId,
                ProposalDocumentPath = request.ProposalDocumentPath,
                Status = ProjectStatus.Submitted,
                SubmittedAt = DateTime.UtcNow,
            };

            var createdProject = await _doctoralProjectRepository.AddAsync(project);

            return new SubmitDoctoralProjectResponse
            {
                Id = createdProject.Id,
                Title = createdProject.Title,
                ResearchArea = createdProject.ResearchArea,
                Status = createdProject.Status.ToString(),
                SubmittedAt = createdProject.SubmittedAt ?? DateTime.UtcNow
            };
        }
    }
}
