using DoctoralManagement.Domain.Interfaces;
using MediatR;

namespace DoctoralManagement.Application.DoctoralProjects.Queries
{
    public class GetDoctoralProjectsByMentorHandler : IRequestHandler<GetDoctoralProjectsByMentorQuery, IEnumerable<GetDoctoralProjectResponse>>
    {
        private readonly IDoctoralProjectRepository _doctoralProjectRepository;

        public GetDoctoralProjectsByMentorHandler(IDoctoralProjectRepository doctoralProjectRepository)
        {
            _doctoralProjectRepository = doctoralProjectRepository;
        }

        public async Task<IEnumerable<GetDoctoralProjectResponse>> Handle(GetDoctoralProjectsByMentorQuery request, CancellationToken cancellationToken)
        {
            var projects = await _doctoralProjectRepository.GetByMentorIdAsync(request.MentorId);

            return projects.Select(project => new GetDoctoralProjectResponse
            {
                Id = project.Id,
                Title = project.Title,
                ResearchArea = project.ResearchArea,
                Status = project.Status.ToString(),
                StudentName = project.Student?.FullName ?? "N/A",
                MentorName = project.Mentor?.FullName ?? "N/A",
                CreatedAt = project.CreatedAt,
                SubmittedAt = project.SubmittedAt
            });
        }
    }
}
