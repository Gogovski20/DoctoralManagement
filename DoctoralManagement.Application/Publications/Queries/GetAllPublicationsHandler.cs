using DoctoralManagement.Application.Dtos;
using DoctoralManagement.Domain.Interfaces;
using MediatR;

namespace DoctoralManagement.Application.Publications.Queries
{
    public class GetAllPublicationsHandler : IRequestHandler<GetAllPublicationsQuery, IEnumerable<GetAllPublicationsResponse>>
    {
        private readonly IPublicationRepository _publicationRepository;

        public GetAllPublicationsHandler(IPublicationRepository publicationRepository)
        {
            _publicationRepository = publicationRepository;
        }

        public async Task<IEnumerable<GetAllPublicationsResponse>> Handle(GetAllPublicationsQuery request, CancellationToken cancellationToken)
        {
            var publications = await _publicationRepository.GetAllAsync();

            return publications.Select(p => new GetAllPublicationsResponse
            {
                Id = p.Id,
                StudentName = p.Student?.FullName ?? "N/A",
                Title = p.Title,
                Journal = p.Journal,
                PublishedOn = p.PublishedOn,
                IsIndexedInScopus = p.IsIndexedInScopus,
                IsIndexedInThomsonReuters = p.IsIndexedInThomsonReuters,
                EctsPoints = p.EctsPoints,
                Document = p.Document == null ? null : new DocumentDto
                {
                    Id = p.Document.Id,
                    FileName = p.Document.FileName
                }
            });
        }
    }
}
