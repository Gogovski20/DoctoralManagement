using MediatR;

namespace DoctoralManagement.Application.Students.Commands
{
    public class UpdateStudentCommand : IRequest<UpdateStudentResponse>
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}
