using MediatR;

namespace DoctoralManagement.Application.Applications.Commands
{
    public class UpdateApplicationCommand : IRequest<UpdateApplicationResponse>
    {
        public int Id { get; set; }
        public int? PreferredMentorId { get; set; }
    }
}
