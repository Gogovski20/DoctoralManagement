using MediatR;

namespace DoctoralManagement.Application.Applications.Commands
{
    public class SubmitApplicationCommand : IRequest<SubmitApplicationResponse>
    {
        public int ApplicationId { get; set; }
    }
}
