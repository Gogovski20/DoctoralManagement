using DoctoralManagement.Application.Dtos;
using MediatR;

namespace DoctoralManagement.Application.Publications.Queries
{
    public class GetPublicationByIdQuery : IRequest<GetPublicationByIdResponse>
    {
        public int PublicationId { get; set; }
    }

    public class GetPublicationByIdResponse
    {
        public int Id { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Journal { get; set; } = string.Empty;
        public DateTime PublishedOn { get; set; }
        public bool IsIndexedInScopus { get; set; }
        public bool IsIndexedInThomsonReuters { get; set; }
        public ActivityDocumentDto? Document { get; set; }
    }
}
