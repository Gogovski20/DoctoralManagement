using MediatR;

namespace DoctoralManagement.Application.Publications.Commands
{
    public class UpdatePublicationCommand : IRequest<PublicationResponse>
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Journal { get; set; } = string.Empty;
        public DateTime PublishedOn { get; set; }
        public string Doi { get; set; } = string.Empty;
        public bool IsIndexedInScopus { get; set; }
        public bool IsIndexedInThomsonReuters { get; set; }
        public int EctsCredits { get; set; }
    }

    public class PublicationResponse { }
}
