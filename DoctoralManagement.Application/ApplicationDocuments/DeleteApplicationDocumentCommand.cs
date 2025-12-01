using MediatR;

namespace DoctoralManagement.Application.ApplicationDocuments
{
    public class DeleteApplicationDocumentCommand : IRequest<bool>
    {
        public int ApplicationId { get; set; }
        public int DocumentId { get; set; }
    }
}
