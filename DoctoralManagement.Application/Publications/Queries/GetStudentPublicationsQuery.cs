using DoctoralManagement.Application.Dtos;
using MediatR;

namespace DoctoralManagement.Application.Publications.Queries
{
    public class GetStudentPublicationsQuery : IRequest<IEnumerable<PublicationResponse>>
    {
        public int StudentId { get; set; }
    }

    public class PublicationResponse
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Journal { get; set; } = string.Empty;
        public DateTime PublishedOn { get; set; }
        public bool IsIndexedInScopus { get; set; }
        public bool IsIndexedInThomsonReuters { get; set; }
        public int EctsPoints { get; set; }
        public DocumentDto? Document { get; set; }
    }
}
