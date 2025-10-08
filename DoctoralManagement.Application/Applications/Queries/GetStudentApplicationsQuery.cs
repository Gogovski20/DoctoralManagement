using MediatR;

namespace DoctoralManagement.Application.Applications.Queries
{
    public class GetStudentApplicationsQuery : IRequest<IEnumerable<GetStudentApplicationsResponse>>
    {
        public int StudentId { get; set; }
    }
}
