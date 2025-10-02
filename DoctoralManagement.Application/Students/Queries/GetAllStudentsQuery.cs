using MediatR;

namespace DoctoralManagement.Application.Students.Queries
{
    public class GetAllStudentsQuery : IRequest<IEnumerable<GetAllStudentsResponse>>
    {
    }
}
