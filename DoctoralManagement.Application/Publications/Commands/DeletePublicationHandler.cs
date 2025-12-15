using DoctoralManagement.Application.Common;
using DoctoralManagement.Application.ECTS.Services;
using DoctoralManagement.Domain.Exceptions;
using DoctoralManagement.Domain.Interfaces;
using MediatR;
using System.Net;

namespace DoctoralManagement.Application.Publications.Commands
{
    public class DeletePublicationHandler : IRequestHandler<DeletePublicationCommand, bool>
    {
        private readonly IPublicationRepository _publicationRepository;
        private readonly IEctsTrackingRepository _ectsRepository;
        private readonly EctsProgressService _progressService;
        private readonly ICurrentUserService _currentUserService;
        private readonly IAuthService _authService;

        public DeletePublicationHandler(IPublicationRepository publicationRepository, IEctsTrackingRepository ectsRepository, EctsProgressService progressService, ICurrentUserService currentUserService, IAuthService authService)
        {
            _publicationRepository = publicationRepository;
            _ectsRepository = ectsRepository;
            _progressService = progressService;
            _currentUserService = currentUserService;
            _authService = authService;
        }

        public async Task<bool> Handle(DeletePublicationCommand request, CancellationToken cancellationToken)
        {
            var publication = await _publicationRepository.GetByIdAsync(request.Id);
            if (publication == null)
                throw new DoctoralManagementException($"Publication with ID {request.Id} not found.", HttpStatusCode.NotFound);

            if (publication.IsApproved == true)
            {
                throw new DoctoralManagementException("You can't delete approved publications.", HttpStatusCode.BadRequest);
            }

            var currentUserId = _currentUserService.UserId;

            var linkedStudentId = await _authService.GetLinkedStudentIdAsync(currentUserId);

            if (linkedStudentId == null || linkedStudentId != publication.StudentId)
            {
                throw new DoctoralManagementException("You can only delete publication for your own account.", HttpStatusCode.Forbidden);
            }

            await _publicationRepository.DeleteAsync(request.Id);

            return true;
        }
    }
}
