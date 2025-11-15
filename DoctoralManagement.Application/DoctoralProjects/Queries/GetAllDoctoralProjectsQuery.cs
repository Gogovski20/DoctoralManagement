using MediatR;

namespace DoctoralManagement.Application.DoctoralProjects.Queries
{
    public class GetAllDoctoralProjectsQuery : IRequest<IEnumerable<GetDoctoralProjectResponse>>
    {
    }
}
