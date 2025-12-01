using DoctoralManagement.Domain.Interfaces;
using MediatR;

namespace DoctoralManagement.Application.DoctoralProjects.Queries
{
    public class GetAllDoctoralProjectsHandler : IRequestHandler<GetAllDoctoralProjectsQuery, IEnumerable<GetDoctoralProjectResponse>>
    {
        private readonly IDoctoralProjectRepository _repository;

        public GetAllDoctoralProjectsHandler(IDoctoralProjectRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<GetDoctoralProjectResponse>> Handle(GetAllDoctoralProjectsQuery request, CancellationToken cancellationToken)
        {
            var projects = await _repository.GetAllWithDetailsAsync();

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
