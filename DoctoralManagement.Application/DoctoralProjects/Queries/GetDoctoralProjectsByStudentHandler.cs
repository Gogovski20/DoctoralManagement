using DoctoralManagement.Domain.Interfaces;
using MediatR;

namespace DoctoralManagement.Application.DoctoralProjects.Queries
{
    public class GetDoctoralProjectsByStudentHandler : IRequestHandler<GetDoctoralProjectsByStudentQuery, IEnumerable<GetDoctoralProjectResponse>>
    {
        private readonly IDoctoralProjectRepository _doctoralProjectRepository;

        public GetDoctoralProjectsByStudentHandler(IDoctoralProjectRepository doctoralProjectRepository)
        {
            _doctoralProjectRepository = doctoralProjectRepository;
        }

        public async Task<IEnumerable<GetDoctoralProjectResponse>> Handle(GetDoctoralProjectsByStudentQuery request, CancellationToken cancellationToken)
        {
            var projects = await _doctoralProjectRepository.GetByStudentIdAsync(request.StudentId);

            return projects.Select(project => new GetDoctoralProjectResponse
            {
                Id = project.Id,
                Title = project.Title,
                ResearchArea = project.ResearchArea,
                Status = project.Status.ToString(),
                StudentName = project.Student?.FullName ?? "N/A",
                MentorName = project.Mentor?.FullName ?? "N/A",
                ProposalDocumentPath = project.ProposalDocumentPath,
                CreatedAt = project.CreatedAt,
                SubmittedAt = project.SubmittedAt
            });
        }
    }
}
