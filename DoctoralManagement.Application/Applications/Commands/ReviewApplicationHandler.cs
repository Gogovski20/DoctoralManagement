using DoctoralManagement.Application.Common;
using DoctoralManagement.Domain.Entities;
using DoctoralManagement.Domain.Exceptions;
using DoctoralManagement.Domain.Interfaces;
using MediatR;
using System.Net;

namespace DoctoralManagement.Application.Applications.Commands
{
    public class ReviewApplicationHandler : IRequestHandler<ReviewApplicationCommand, ReviewApplicationResponse>
    {
        private readonly IApplicationRepository _applicationRepository;
        private readonly IDoctoralProgramRepository _doctoralProgramRepository;
        private readonly IStudentRepository _studentRepository;
        private readonly IEctsTrackingRepository _ectsTrackingRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IAuthService _authService;

        public ReviewApplicationHandler(IApplicationRepository applicationRepository, IDoctoralProgramRepository doctoralProgramRepository, IStudentRepository studentRepository, IEctsTrackingRepository ectsTrackingRepository, ICurrentUserService currentUserService, IAuthService authService)
        {
            _applicationRepository = applicationRepository;
            _doctoralProgramRepository = doctoralProgramRepository;
            _studentRepository = studentRepository;
            _ectsTrackingRepository = ectsTrackingRepository;
            _currentUserService = currentUserService;
            _authService = authService;
        }

        public async Task<ReviewApplicationResponse> Handle(ReviewApplicationCommand request, CancellationToken cancellationToken)
        {
            var application = await _applicationRepository.GetByIdWithDetailsAsync(request.Id);

            if (application == null)
            {
                throw new DoctoralManagementException($"Application with ID: {request.Id} not found", HttpStatusCode.NotFound);
            }

            var currentUserRole = _currentUserService.Role;
            
            if (currentUserRole != "Admin")
            {
                throw new DoctoralManagementException("Only admin can review the applications.");
            }

            if (!IsValidStatusTransition(application.ApplicationStatus, request.NewStatus))
            {
                throw new DoctoralManagementException($"Invalid status transition from {application.ApplicationStatus} to {request.NewStatus}", HttpStatusCode.BadRequest);
            }

            if (request.NewStatus == ApplicationStatus.FinalAccepted)
            {
                var program = await _doctoralProgramRepository.GetByIdAsync(application.DoctoralProgramId);
                
                if (program.CurrentStudentsCount >= program.AvailableSlots)
                {
                    throw new DoctoralManagementException($"Cannot accept application - program '{program.Name}' is full. Current: {program.CurrentStudentsCount}/{program.AvailableSlots}", HttpStatusCode.BadRequest);
                }

                var student = await _studentRepository.GetByIdAsync(application.StudentId);

                program.CurrentStudentsCount += 1;
                await _doctoralProgramRepository.UpdateAsync(program);

                student.DoctoralProgramId = application.DoctoralProgramId;

                if (student.CurrentSemester == 0)
                {
                    student.CurrentSemester = 1;
                }

                await _studentRepository.UpdateAsync(student);

                var existingEcts = await _ectsTrackingRepository.GetByStudentIdAsync(student.Id);
                if (existingEcts == null)
                {
                    var newEctsTracking = new ECTSTracking
                    {
                        StudentId = student.Id,
                        OrganizedAcademicTraining = 0,
                        IndependentResearchProject = 0,
                        InternationalMobility = 0,
                        TeachingActivities = 0,
                        Publications = 0,
                        ThesisDefence = 0
                    };
                    await _ectsTrackingRepository.CreateAsync(newEctsTracking);
                }
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
