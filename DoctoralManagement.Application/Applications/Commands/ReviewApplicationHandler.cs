using DoctoralManagement.Domain.Entities;
using DoctoralManagement.Domain.Interfaces;
using MediatR;

namespace DoctoralManagement.Application.Applications.Commands
{
    public class ReviewApplicationHandler : IRequestHandler<ReviewApplicationCommand, ReviewApplicationResponse>
    {
        private readonly IApplicationRepository _applicationRepository;

        public ReviewApplicationHandler(IApplicationRepository applicationRepository)
        {
            _applicationRepository = applicationRepository;
        }

        public async Task<ReviewApplicationResponse> Handle(ReviewApplicationCommand request, CancellationToken cancellationToken)
        {
            var application = await _applicationRepository.GetByIdAsync(request.Id);

            if (application == null)
            {
                throw new Exception($"Application with ID: {request.Id} not found");
            }

            if (!IsValidStatusTransition(application.ApplicationStatus, request.NewStatus))
            {
                throw new Exception($"Invalid status transition from {application.ApplicationStatus} to {request.NewStatus}");
            }

            application.ApplicationStatus = request.NewStatus;
            application.HasRequiredPublications = request.HasRequiredPublications;

            if (request.NewStatus == ApplicationStatus.FinalAccepted || request.NewStatus == ApplicationStatus.Rejected)
            {
                application.DecisionDate = DateTime.UtcNow;
            }

            await _applicationRepository.UpdateAsync(application);

            return new ReviewApplicationResponse
            {
                Id = application.Id,
                StudentId = application.StudentId,
                DoctoralProgramId = application.DoctoralProgramId,
                ApplicationStatus = application.ApplicationStatus,
                ReviewComments = request.ReviewComments,
                HasRequiredPublications = application.HasRequiredPublications,
                DecisionDate = application.DecisionDate
            };
        }

        private bool IsValidStatusTransition(ApplicationStatus currentStatus, ApplicationStatus newStatus)
        {
            var validTransitions = new Dictionary<ApplicationStatus, List<ApplicationStatus>>
            {
                [ApplicationStatus.Submitted] = new() { ApplicationStatus.UnderReview, ApplicationStatus.Rejected },
                [ApplicationStatus.UnderReview] = new() { ApplicationStatus.PreliminaryAccepted, ApplicationStatus.Rejected },
                [ApplicationStatus.PreliminaryAccepted] = new() { ApplicationStatus.FinalAccepted, ApplicationStatus.Rejected },
                [ApplicationStatus.Draft] = new() { ApplicationStatus.Submitted }
            };

            return validTransitions.ContainsKey(currentStatus) &&
                validTransitions[currentStatus].Contains(newStatus);
        }
    }
}
