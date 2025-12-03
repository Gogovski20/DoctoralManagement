using MediatR;

namespace DoctoralManagement.Application.Publications.Commands
{
    public class CreatePublicationCommand : IRequest<CreatePublicationResponse>
    {
        public int StudentId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Journal { get; set; } = string.Empty;
        public DateTime PublishedOn { get; set; }
        public string Doi { get; set; } = string.Empty;
        public bool IsIndexedInScopus { get; set; }
        public bool IsIndexedInThomsonReuters { get; set; }
    }

    public class CreatePublicationResponse
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public string Title { get; set; } = string.Empty;
    }
}
