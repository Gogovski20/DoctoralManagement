using DoctoralManagement.Application.Common;
using DoctoralManagement.Application.ECTS.Services;
using DoctoralManagement.Domain.Exceptions;
using DoctoralManagement.Domain.Interfaces;
using MediatR;
using System.Net;

namespace DoctoralManagement.Application.Mobilities.Commands
{
    public class DeleteMobilityHandler : IRequestHandler<DeleteMobilityCommand, bool>
    {
        private readonly IMobilityRepository _mobilityRepository;
        private readonly IEctsTrackingRepository _ectsRepository;
        private readonly EctsProgressService _ectsProgressService;
        private readonly ICurrentUserService _currentUserService;
        private readonly IAuthService _authService;

        public DeleteMobilityHandler(IMobilityRepository mobilityRepository, IEctsTrackingRepository ectsRepository, EctsProgressService ectsProgressService, ICurrentUserService currentUserService, IAuthService authService)
        {
            _mobilityRepository = mobilityRepository;
            _ectsRepository = ectsRepository;
            _ectsProgressService = ectsProgressService;
            _currentUserService = currentUserService;
            _authService = authService;
        }

        public async Task<bool> Handle(DeleteMobilityCommand request, CancellationToken cancellationToken)
        {
            var mobility = await _mobilityRepository.GetByIdAsync(request.Id);
            if (mobility == null)
                throw new DoctoralManagementException($"Mobility with ID {request.Id} not found.", HttpStatusCode.NotFound);

            if (mobility.IsApproved == true)
            {
                throw new DoctoralManagementException("You can't delete approved mobilities.", HttpStatusCode.BadRequest);
            }

            var currentUserId = _currentUserService.UserId;

            var linkedStudentId = await _authService.GetLinkedStudentIdAsync(currentUserId);

            if (linkedStudentId == null || linkedStudentId != mobility.StudentId)
            {
                throw new DoctoralManagementException("You can only delete mobility for your own account.", HttpStatusCode.Forbidden);
            }

            await _mobilityRepository.DeleteAsync(request.Id);

            return true;
        }
    }
}
