using DoctoralManagement.Domain.Interfaces;
using MediatR;

namespace DoctoralManagement.Application.Publications.Queries
{
    public class GetStudentPublicationsHandler : IRequestHandler<GetStudentPublicationsQuery, IEnumerable<PublicationResponse>>
    {
        private readonly IPublicationRepository _publicationRepository;

        public GetStudentPublicationsHandler(IPublicationRepository publicationRepository)
        {
            _publicationRepository = publicationRepository;
        }

        public async Task<IEnumerable<PublicationResponse>> Handle(GetStudentPublicationsQuery request, CancellationToken cancellationToken)
        {
            var publications = await _publicationRepository.GetByStudentIdAsync(request.StudentId);

            return publications.Select(p => new PublicationResponse
            {
                Id = p.Id,
                Title = p.Title,
                Journal = p.Journal,
                PublishedOn = p.PublishedOn,
                IsIndexedInScopus = p.IsIndexedInScopus,
                IsIndexedInThomsonReuters = p.IsIndexedInThomsonReuters,
                EctsPoints = p.EctsPoints
            });
        }
    }
}
