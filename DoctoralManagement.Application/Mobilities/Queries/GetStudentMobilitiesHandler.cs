using DoctoralManagement.Application.Common;
using DoctoralManagement.Application.Dtos;
using DoctoralManagement.Domain.Exceptions;
using DoctoralManagement.Domain.Interfaces;
using MediatR;
using System.Net;

namespace DoctoralManagement.Application.Mobilities.Queries
{
    public class GetStudentMobilitiesHandler : IRequestHandler<GetStudentMobilitiesQuery, IEnumerable<MobilityResponse>>
    {
        private readonly IMobilityRepository _mobilityRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IAuthService _authService;

        public GetStudentMobilitiesHandler(IMobilityRepository mobilityRepository, ICurrentUserService currentUserService, IAuthService authService)
        {
            _mobilityRepository = mobilityRepository;
            _currentUserService = currentUserService;
            _authService = authService;
        }

        public async Task<IEnumerable<MobilityResponse>> Handle(GetStudentMobilitiesQuery request, CancellationToken cancellationToken)
        {
            var currentUserId = _currentUserService.UserId;
            var currentUserRole = _currentUserService.Role;
            var linkedStudentId = await _authService.GetLinkedStudentIdAsync(currentUserId);

            bool isOwner = linkedStudentId == request.StudentId;
            bool isAdmin = currentUserRole == "Admin";

            if (!isOwner && !isAdmin)
            {
                throw new DoctoralManagementException(
                    "You can only view your own mobilities.",
                    HttpStatusCode.Forbidden);
            }

            var mobilities = await _mobilityRepository.GetByStudentIdAsync(request.StudentId);

            return mobilities.Select(m => new MobilityResponse
            {
                Id = m.Id,
                Institution = m.Institution,
                Country = m.Country,
                StartDate = m.StartDate,
                EndDate = m.EndDate,
                Document = m.Document == null ? null : new DocumentDto
                {
                    Id = m.Document.Id,
                    FileName = m.Document.FileName
                }
            }).ToList();
        }
    }
}
