using MediatR;

namespace DoctoralManagement.Application.DoctoralPrograms.Commands
{
    public class DeleteDoctoralProgramCommand : IRequest<DeleteDoctoralProgramResponse>
    {
        public int Id { get; set; }
    }
}
