using MediatR;

namespace DoctoralManagement.Application.Mentors.Commands
{
    public class DeleteMentorCommand : IRequest<bool>
    {
        public int Id { get; set; }
    }
}
