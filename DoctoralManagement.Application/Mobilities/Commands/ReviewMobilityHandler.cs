using DoctoralManagement.Application.Common;
using DoctoralManagement.Application.ECTS.Services;
using DoctoralManagement.Domain.Exceptions;
using DoctoralManagement.Domain.Interfaces;
using MediatR;
using System.Net;

namespace DoctoralManagement.Application.Mobilities.Commands
{
    public class ReviewMobilityHandler : IRequestHandler<ReviewMobilityCommand, ReviewMobilityResponse>
    {
        private readonly IMobilityRepository _mobilityRepository;
        private readonly IActivityDocumentRepository _activityDocumentRepository;
        private readonly IEctsTrackingRepository _ectsTrackingRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly EctsProgressService _ectsProgressService;

        public ReviewMobilityHandler(IMobilityRepository mobilityRepository, IActivityDocumentRepository activityDocumentRepository, IEctsTrackingRepository ectsTrackingRepository, ICurrentUserService currentUserService, EctsProgressService ectsProgressService)
        {
            _mobilityRepository = mobilityRepository;
            _activityDocumentRepository = activityDocumentRepository;
            _ectsTrackingRepository = ectsTrackingRepository;
            _currentUserService = currentUserService;
            _ectsProgressService = ectsProgressService;
        }

        public async Task<ReviewMobilityResponse> Handle(ReviewMobilityCommand request, CancellationToken cancellationToken)
        {
            var mobility = await _mobilityRepository.GetByIdAsync(request.MobilityId)
                ?? throw new DoctoralManagementException("Mobility not found", HttpStatusCode.NotFound);

            var currentUserId = _currentUserService.UserId;
            var currentUserRole = _currentUserService.Role;

            if (currentUserRole != "Admin")
            {
                throw new DoctoralManagementException("Only admins can review mobilities.", HttpStatusCode.Forbidden);
            }

            if (mobility.IsApproved)
            {
                throw new DoctoralManagementException("This mobility is already approved.", HttpStatusCode.BadRequest);
            }

            var mobilityDocument = await _activityDocumentRepository.GetByMobilityIdAsync(mobility.Id);
            if (mobilityDocument == null)
            {
                throw new DoctoralManagementException("Mobility cannot be reviewed cause proof document is missing.", HttpStatusCode.BadRequest);
            }

            if (request.IsApproved)
            {
                mobility.IsApproved = true;
                mobility.EctsPoints = request.EctsAwarded;
                mobilityDocument.Status = Domain.Entities.DocumentStatus.Approved;
                mobilityDocument.ReviewComment = request.ReviewComments;
                mobilityDocument.ReviewedAt = DateTime.UtcNow;
                mobilityDocument.ReviewedBy = currentUserId;

                var ectsTracking = await _ectsTrackingRepository.GetByStudentIdAsync(mobility.StudentId);
                if (ectsTracking != null)
                {
                    ectsTracking.InternationalMobility += mobility.EctsPoints;
                    if (ectsTracking.InternationalMobility > 6)
                    {
                        ectsTracking.InternationalMobility = 6;
                    }
                    await _ectsTrackingRepository.UpdateAsync(ectsTracking);
                    await _ectsProgressService.UpdateStudentSemesterAsync(mobility.StudentId, ectsTracking.TotalECTS);
                }
            }
            else
            {
                mobility.IsApproved = false;
                mobilityDocument.Status = Domain.Entities.DocumentStatus.Rejected;
                mobilityDocument.ReviewComment = request.ReviewComments;
                mobilityDocument.ReviewedAt = DateTime.UtcNow;
                mobilityDocument.ReviewedBy = currentUserId;
            }

            await _mobilityRepository.UpdateAsync(mobility);
            await _activityDocumentRepository.UpdateAsync(mobilityDocument);
            return new ReviewMobilityResponse
            {
                MobilityId = mobility.Id,
                IsApproved = mobility.IsApproved,
                EctsAwarded = mobility.EctsPoints,
                Document = new Dtos.ActivityDocumentDto
                {
                    Id = mobilityDocument.Id,
                    Type = mobilityDocument.DocumentType,
                    FileName = mobilityDocument.FileName,
                    FilePath = mobilityDocument.FilePath,
                    ContentType = mobilityDocument.ContentType,
                    UploadedAt = mobilityDocument.UploadedAt,
                    ReviewComment = mobilityDocument.ReviewComment,
                    ReviewedBy = (int)mobilityDocument.ReviewedBy,
                    ReviewedAt = (DateTime)mobilityDocument.ReviewedAt
                }
            };
        }
    }
}
