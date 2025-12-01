using MediatR;

namespace DoctoralManagement.Application.Applications.Commands
{
    public class CreateApplicationCommand : IRequest<CreateApplicationResponse>
    {
        public int StudentId { get; set; }
        public int DoctoralProgramId { get; set; }
        public int? PreferredMentorId { get; set; }
    }
}
