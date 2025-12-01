using DoctoralManagement.Application.Common;
using DoctoralManagement.Application.Dtos;
using DoctoralManagement.Application.ECTS.Services;
using DoctoralManagement.Domain.Entities;
using DoctoralManagement.Domain.Interfaces;
using MediatR;

namespace DoctoralManagement.Application.ConferenceParticipations.Commands
{
    public class ReviewConferenceHandler : IRequestHandler<ReviewConferenceCommand, ReviewConferenceResponse>
    {
        private readonly IConferenceParticipationRepository _conferenceRepository;
        private readonly IActivityDocumentRepository _activityDocumentRepository;
        private readonly IEctsTrackingRepository _ectsTrackingRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly EctsProgressService _ectsProgressService;

        public ReviewConferenceHandler(IConferenceParticipationRepository conferenceRepository, IActivityDocumentRepository activityDocumentRepository, IEctsTrackingRepository ectsTrackingRepository, ICurrentUserService currentUserService, EctsProgressService ectsProgressService)
        {
            _conferenceRepository = conferenceRepository;
            _activityDocumentRepository = activityDocumentRepository;
            _ectsTrackingRepository = ectsTrackingRepository;
            _currentUserService = currentUserService;
            _ectsProgressService = ectsProgressService;
        }

        public async Task<ReviewConferenceResponse> Handle(ReviewConferenceCommand request, CancellationToken cancellationToken)
        {
            var currentUserId = _currentUserService.UserId;
            var currentUserRole = _currentUserService.Role;

            if (currentUserRole != "Admin")
            {
                throw new Exception("Only admins can review mobilities.");
            }

            var conference = await _conferenceRepository.GetByIdAsync(request.ConferenceId)
                ?? throw new Exception("Conference participation not found.");

            if (conference.IsApproved)
            {
                throw new Exception("Conference participation has already been reviewed.");
            }

            var conferenceDocument = await _activityDocumentRepository.GetByConferenceIdAsync(request.ConferenceId);
            if (conferenceDocument == null)
            {
                throw new Exception("No document found for the specified conference participation.");
            }

            if (request.IsApproved)
            {
                conference.IsApproved = true;
                conferenceDocument.Status = DocumentStatus.Approved;
                conferenceDocument.ReviewComment = request.ReviewComments;
                conferenceDocument.ReviewedAt = DateTime.UtcNow;
                conferenceDocument.ReviewedBy = currentUserId;

                var ectsTracking = await _ectsTrackingRepository.GetByStudentIdAsync(conference.StudentId);
                if (ectsTracking != null)
                {
                    ectsTracking.TeachingActivities += conference.EctsAwarded;
                    if (ectsTracking.TeachingActivities > 18)
                    {
                        ectsTracking.TeachingActivities = 18;
                    }
                    await _ectsTrackingRepository.UpdateAsync(ectsTracking);
                    await _ectsProgressService.UpdateStudentSemesterAsync(conference.StudentId, ectsTracking.TotalECTS);
                }
            }
            else
            {
                conference.IsApproved = false;
                conferenceDocument.Status = DocumentStatus.Rejected;
                conferenceDocument.ReviewComment = request.ReviewComments;
                conferenceDocument.ReviewedAt = DateTime.UtcNow;
                conferenceDocument.ReviewedBy = currentUserId;
            }
            await _conferenceRepository.UpdateAsync(conference);
            await _activityDocumentRepository.UpdateAsync(conferenceDocument);

            return new ReviewConferenceResponse
            {
                ConferenceId = conference.Id,
                IsApproved = conference.IsApproved,
                Document = new ActivityDocumentDto
                {
                    Id = conferenceDocument.Id,
                    Type = conferenceDocument.DocumentType,
                    FileName = conferenceDocument.FileName,
                    FilePath = conferenceDocument.FilePath,
                    ContentType = conferenceDocument.ContentType,
                    UploadedAt = conferenceDocument.UploadedAt,
                    ReviewComment = conferenceDocument.ReviewComment,
                    ReviewedBy = (int)conferenceDocument.ReviewedBy,
                    ReviewedAt = (DateTime)conferenceDocument.ReviewedAt
                }
            };
        }
    }
}
