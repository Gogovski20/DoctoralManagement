using DoctoralManagement.Domain.Entities;
using DoctoralManagement.Domain.Interfaces;
using MediatR;

namespace DoctoralManagement.Application.DoctoralProjects.Commands
{
    public class CreateDoctoralProjectDraftHandler : IRequestHandler<CreateDoctoralProjectDraftCommand, CreateDoctoralProjectDraftResponse>
    {
        private readonly IDoctoralProjectRepository _doctoralProjectRepository;
        private readonly IStudentRepository _studentRepository;
        private readonly IMentorRepository _mentorRepository;

        public CreateDoctoralProjectDraftHandler(IDoctoralProjectRepository doctoralProjectRepository, IStudentRepository studentRepository, IMentorRepository mentorRepository)
        {
            _doctoralProjectRepository = doctoralProjectRepository;
            _studentRepository = studentRepository;
            _mentorRepository = mentorRepository;
        }

        public async Task<CreateDoctoralProjectDraftResponse> Handle(CreateDoctoralProjectDraftCommand request, CancellationToken cancellationToken)
        {
            var student = await _studentRepository.GetByIdAsync(request.StudentId)
                ?? throw new Exception($"Student with id {request.StudentId} not found");

            var mentor = await _mentorRepository.GetByIdAsync(request.MentorId)
                ?? throw new Exception($"Mentor with id {request.MentorId} not found");

            var project = new DoctoralProject
            {
                Title = request.Title,
                ResearchArea = request.ResearchArea,
                StudentId = request.StudentId,
                MentorId = request.MentorId,
                ProposalDocumentPath = request.ProposalDocumentPath,
                Status = ProjectStatus.Draft,
                CreatedAt = DateTime.UtcNow
            };

            var created = await _doctoralProjectRepository.AddAsync(project);

            return new CreateDoctoralProjectDraftResponse
            {
                Id = created.Id,
                Title = created.Title,
                ResearchArea = created.ResearchArea,
                Status = created.Status.ToString(),
                ProposalDocumentPath = created.ProposalDocumentPath,
                CreatedAt = created.CreatedAt
            };
        }
    }
}
