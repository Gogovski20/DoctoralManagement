using MediatR;

namespace DoctoralManagement.Application.DoctoralPrograms.Queries
{
    public class GetAllDoctoralProgramsQuery : IRequest<IEnumerable<GetAllDoctoralProgramsResponse>>
    {
    }
}
