using DoctoralManagement.Application.Common;
using DoctoralManagement.Application.ECTS.Services;
using DoctoralManagement.Domain.Interfaces;
using MediatR;

namespace DoctoralManagement.Application.Publications.Commands
{
    public class ReviewPublicationHandler : IRequestHandler<ReviewPublicationCommand, ReviewPublicationResponse>
    {
        private readonly IPublicationRepository _publicationRepository;
        private readonly IActivityDocumentRepository _activityDocumentRepository;
        private readonly IEctsTrackingRepository _ectsTrackingRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly EctsProgressService _ectsProgressService;

        public ReviewPublicationHandler(IPublicationRepository publicationRepository, IActivityDocumentRepository activityDocumentRepository, IEctsTrackingRepository ectsTrackingRepository, ICurrentUserService currentUserService, EctsProgressService ectsProgressService)
        {
            _publicationRepository = publicationRepository;
            _activityDocumentRepository = activityDocumentRepository;
            _ectsTrackingRepository = ectsTrackingRepository;
            _currentUserService = currentUserService;
            _ectsProgressService = ectsProgressService;
        }

        public async Task<ReviewPublicationResponse> Handle(ReviewPublicationCommand request, CancellationToken cancellationToken)
        {
            var currentUserId = _currentUserService.UserId;
            var currentUserRole = _currentUserService.Role;

            if (currentUserRole != "Admin")
            {
                throw new Exception("Only admins can review publications.");
            }

            var publication = await _publicationRepository.GetByIdAsync(request.PublicationId)
                ?? throw new Exception("Publication not found");

            if (publication.IsApproved)
            {
                throw new Exception("This publication is already approved.");
            }

            var publicationDocument = await _activityDocumentRepository.GetByPublicationIdAsync(publication.Id);
            if (publicationDocument == null) 
            {
                throw new Exception("Publication cannot be reviewed cause proof document is missing.");
            }

            if (request.IsApproved)
            {
                publication.IsApproved = true;
                publicationDocument.Status = Domain.Entities.DocumentStatus.Approved;
                publicationDocument.ReviewComment = request.ReviewComments;
                publicationDocument.ReviewedAt = DateTime.UtcNow;
                publicationDocument.ReviewedBy = currentUserId;

                var ectsTracking = await _ectsTrackingRepository.GetByStudentIdAsync(publication.StudentId);
                if (ectsTracking != null)
                {
                    ectsTracking.Publications += publication.EctsPoints;
                    if (ectsTracking.Publications > 27)
                    {
                        ectsTracking.Publications = 27;
                    }
                    await _ectsTrackingRepository.UpdateAsync(ectsTracking);
                    await _ectsProgressService.UpdateStudentSemesterAsync(publication.StudentId, ectsTracking.TotalECTS);
                }
            }
            else
            {
                publication.IsApproved = false;
                publicationDocument.Status = Domain.Entities.DocumentStatus.Rejected;
                publicationDocument.ReviewComment = request.ReviewComments;
                publicationDocument.ReviewedAt = DateTime.UtcNow;
                publicationDocument.ReviewedBy = currentUserId;
            }

            await _publicationRepository.UpdateAsync(publication);
            await _activityDocumentRepository.UpdateAsync(publicationDocument);
            return new ReviewPublicationResponse
            {
                PublicationId = publication.Id,
                IsApproved = publication.IsApproved,
                Document = new Dtos.ActivityDocumentDto
                {
                    Id = publicationDocument.Id,
                    Type = publicationDocument.DocumentType,
                    FileName = publicationDocument.FileName,
                    FilePath = publicationDocument.FilePath,
                    ContentType = publicationDocument.ContentType,
                    UploadedAt = publicationDocument.UploadedAt,
                    ReviewComment = publicationDocument.ReviewComment,
                    ReviewedBy = (int)publicationDocument.ReviewedBy,
                    ReviewedAt = (DateTime)publicationDocument.ReviewedAt
                }
            };
        }
    }
}
