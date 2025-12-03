using DoctoralManagement.Application.Common;
using DoctoralManagement.Domain.Exceptions;
using DoctoralManagement.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Net;

namespace DoctoralManagement.Application.ECTS.Queries
{
    public class GetStudentEctsStatusHandler : IRequestHandler<GetStudentEctsStatusQuery, StudentEctsStatusResponse>
    {
        private readonly IEctsTrackingRepository _repository;
        private readonly IStudentRepository _studentRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IAuthService _authService;
        private readonly ILogger<GetStudentEctsStatusHandler> _logger;

        public GetStudentEctsStatusHandler(IEctsTrackingRepository repository, IStudentRepository studentRepository, ICurrentUserService currentUserService, IAuthService authService, ILogger<GetStudentEctsStatusHandler> logger)
        {
            _repository = repository;
            _studentRepository = studentRepository;
            _currentUserService = currentUserService;
            _authService = authService;
            _logger = logger;
        }

        public async Task<StudentEctsStatusResponse> Handle(GetStudentEctsStatusQuery request, CancellationToken cancellationToken)
        {
            var currentUserId = _currentUserService.UserId;
            var currentUserRole = _currentUserService.Role;
            var linkedStudentId = await _authService.GetLinkedStudentIdAsync(currentUserId);
            var linkedMentorId = await _authService.GetLinkedMentorIdAsync(currentUserId);

            bool isOwner = linkedStudentId == request.StudentId;
            bool isAdmin = currentUserRole == "Admin";
            bool isMentor = linkedMentorId.HasValue; 
            bool isSecretary = currentUserRole == "Secretary";

            if (!isOwner && !isAdmin && !isMentor && !isSecretary)
            {
                throw new DoctoralManagementException(
                    "You can only view your own ECTS status.",
                    HttpStatusCode.Forbidden);
            }

            var student = await _studentRepository.GetByIdAsync(request.StudentId)
                ?? throw new DoctoralManagementException($"Student with id {request.StudentId} not found.", HttpStatusCode.NotFound);

            var ectsTracking = await _repository.GetByStudentIdAsync(request.StudentId)
                ?? throw new DoctoralManagementException($"ECTS tracking record not found for student {request.StudentId}.", HttpStatusCode.NotFound);

            int totalEcts = ectsTracking.TotalECTS;
            double progressPercent = (totalEcts / 180.0) * 100.0;

            _logger.LogInformation(
                "{Role} {UserId} viewed ECTS status for student {StudentId}",
                currentUserRole, currentUserId, request.StudentId);

            return new StudentEctsStatusResponse
            {
                StudentId = request.StudentId,
                OrganizedAcademicTraining = ectsTracking.OrganizedAcademicTraining,
                IndependentResearchProject = ectsTracking.IndependentResearchProject,
                InternationalMobility = ectsTracking.InternationalMobility,
                TeachingActivities = ectsTracking.TeachingActivities,
                Publications = ectsTracking.Publications,
                ThesisDefence = ectsTracking.ThesisDefence,
                TotalEcts = totalEcts,
                CurrentSemester = student.CurrentSemester,
                IsCompleted = ectsTracking.IsCompleted,
                ProgressPercent = progressPercent
            };
        }
    }
}
