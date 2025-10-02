using MediatR;

namespace DoctoralManagement.Application.Students.Queries
{
    public class GetStudentByIdQuery : IRequest<GetStudentByIdResponse>
    {
        public int Id { get; set; }
    }
}
