using MediatR;

namespace DoctoralManagement.Application.Applications.Queries
{
    public class GetApplicationByIdQuery : IRequest<GetApplicationByIdResponse>
    {
        public int Id { get; set; }
    }
}
