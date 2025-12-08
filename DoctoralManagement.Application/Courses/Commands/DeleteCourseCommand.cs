using MediatR;

namespace DoctoralManagement.Application.Courses.Commands
{
    public class DeleteCourseCommand : IRequest<bool>
    {
        public int Id { get; set; }
    }
}
