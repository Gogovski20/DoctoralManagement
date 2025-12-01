using MediatR;

namespace DoctoralManagement.Application.ActivityDocuments
{
    public class DeleteActivityDocumentCommand : IRequest<bool>
    {
        public int ActivityDocumentId { get; set; }
        public ActivityType ActivityType { get; set; }
        public int ActivityId { get; set; }
    }
}
