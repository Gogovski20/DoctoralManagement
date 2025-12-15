using DoctoralManagement.Application.Dtos;
using DoctoralManagement.Domain.Interfaces;
using MediatR;

namespace DoctoralManagement.Application.DoctoralProjects.Queries
{
    public class GetDefenseEligibleProjectsHandler : IRequestHandler<GetDefenseEligibleProjectsQuery, List<DefenseEligibleProjectDto>>
    {
        private readonly IDoctoralProjectRepository _repository;

        public GetDefenseEligibleProjectsHandler(IDoctoralProjectRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<DefenseEligibleProjectDto>> Handle(GetDefenseEligibleProjectsQuery request, CancellationToken cancellationToken)
        {
            var projects = await _repository.GetAllWithDetailsAsync();

            return projects
                .Where(p => p.Status == Domain.Entities.ProjectStatus.DefenseUnderReview)
                .Select(p => new DefenseEligibleProjectDto
                {
                    Id = p.Id,
                    Title = p.Title,
                    StudentName = p.Student?.FullName ?? "N/A"
                }).ToList();
        }
    }
}
