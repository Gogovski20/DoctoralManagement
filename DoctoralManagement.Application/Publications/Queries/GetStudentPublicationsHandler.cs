using DoctoralManagement.Application.Common;
using DoctoralManagement.Application.Dtos;
using DoctoralManagement.Domain.Exceptions;
using DoctoralManagement.Domain.Interfaces;
using MediatR;
using System.Net;

namespace DoctoralManagement.Application.Publications.Queries
{
    public class GetStudentPublicationsHandler : IRequestHandler<GetStudentPublicationsQuery, IEnumerable<PublicationResponse>>
    {
        private readonly IPublicationRepository _publicationRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IAuthService _authService;

        public GetStudentPublicationsHandler(IPublicationRepository publicationRepository, ICurrentUserService currentUserService, IAuthService authService)
        {
            _publicationRepository = publicationRepository;
            _currentUserService = currentUserService;
            _authService = authService;
        }

        public async Task<IEnumerable<PublicationResponse>> Handle(GetStudentPublicationsQuery request, CancellationToken cancellationToken)
        {
            var currentUserId = _currentUserService.UserId;
            var currentUserRole = _currentUserService.Role;
            var linkedStudentId = await _authService.GetLinkedStudentIdAsync(currentUserId);

            bool isOwner = linkedStudentId == request.StudentId;
            bool isAdmin = currentUserRole == "Admin";

            if (!isOwner && !isAdmin)
            {
                throw new DoctoralManagementException(
                    "You can only view your own conference participations.",
                    HttpStatusCode.Forbidden);
            }

            var publications = await _publicationRepository.GetByStudentIdAsync(request.StudentId);

            return publications.Select(p => new PublicationResponse
            {
                Id = p.Id,
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
