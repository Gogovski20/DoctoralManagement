using MediatR;

namespace DoctoralManagement.Application.Students.Commands
{
    public class DeleteStudentCommand : IRequest<DeleteStudentResponse>
    {
        public int Id { get; set; }
    }
}
