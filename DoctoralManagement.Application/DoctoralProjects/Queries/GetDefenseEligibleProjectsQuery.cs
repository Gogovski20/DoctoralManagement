using DoctoralManagement.Application.Dtos;
using MediatR;

namespace DoctoralManagement.Application.DoctoralProjects.Queries
{
    public class GetDefenseEligibleProjectsQuery : IRequest<List<DefenseEligibleProjectDto>>
    {
    }
}
