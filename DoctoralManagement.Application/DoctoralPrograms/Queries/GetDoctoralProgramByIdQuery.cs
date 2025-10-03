using MediatR;

namespace DoctoralManagement.Application.DoctoralPrograms.Queries
{
    public class GetDoctoralProgramByIdQuery : IRequest<GetDoctoralProgramByIdResponse>
    {
        public int Id { get; set; }
    }
}
