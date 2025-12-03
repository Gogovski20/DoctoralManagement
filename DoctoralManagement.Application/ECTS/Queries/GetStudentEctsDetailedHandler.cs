using DoctoralManagement.Application.Common;
using DoctoralManagement.Domain.Exceptions;
using DoctoralManagement.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Net;

namespace DoctoralManagement.Application.ECTS.Queries
{
    public class GetStudentEctsDetailedHandler : IRequestHandler<GetStudentEctsDetailedQuery, StudentEctsDetailedResponse>
    {
        private readonly IEctsTrackingRepository _ectsTrackingRepository;
        private readonly IStudentRepository _studentRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IAuthService _authService;
        private readonly ILogger<GetStudentEctsDetailedHandler> _logger;

        public GetStudentEctsDetailedHandler(IEctsTrackingRepository ectsTrackingRepository, IStudentRepository studentRepository, ICurrentUserService currentUserService, IAuthService authService, ILogger<GetStudentEctsDetailedHandler> logger)
        {
            _ectsTrackingRepository = ectsTrackingRepository;
            _studentRepository = studentRepository;
            _currentUserService = currentUserService;
            _authService = authService;
            _logger = logger;
        }

        public async Task<StudentEctsDetailedResponse> Handle(GetStudentEctsDetailedQuery request, CancellationToken cancellationToken)
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
                    "You can only view your own detailed ECTS breakdown.",
                    HttpStatusCode.Forbidden);
            }

            var student = await _studentRepository.GetByIdAsync(request.StudentId)
                ?? throw new Exception($"Student with id {request.StudentId} not found.");

            var ectsTracking = await _ectsTrackingRepository.GetByStudentIdAsync(request.StudentId)
                ?? throw new Exception($"ECTS tracking record not found for student {request.StudentId}.");

            int totalEcts = ectsTracking.TotalECTS;
            double totalProgressPercent = (totalEcts / 180.0) * 100;

            var categories = new List<EctsCategoryBreakdown>
            {
                new EctsCategoryBreakdown
                {
                    CategoryName = "Organized Academic Training",
                    Required = 42,
                    Awarded = ectsTracking.OrganizedAcademicTraining,
                    Remaining = Math.Max(0, 42 - ectsTracking.OrganizedAcademicTraining),
                    IsComplete = ectsTracking.OrganizedAcademicTraining >= 42,
                    ProgressPercent = (ectsTracking.OrganizedAcademicTraining / 42.0) * 100.0
                },
                new EctsCategoryBreakdown
                {
                    CategoryName = "Independent Research Project",
                    Required = 41,
                    Awarded = ectsTracking.IndependentResearchProject,
                    Remaining = Math.Max(0, 41 - ectsTracking.IndependentResearchProject),
                    IsComplete = ectsTracking.IndependentResearchProject >= 41,
                    ProgressPercent = (ectsTracking.IndependentResearchProject / 41.0) * 100.0
                },
                new EctsCategoryBreakdown
                {
                    CategoryName = "International Mobility",
                    Required = 6,
                    Awarded = ectsTracking.InternationalMobility,
                    Remaining = Math.Max(0, 6 - ectsTracking.InternationalMobility),
                    IsComplete = ectsTracking.InternationalMobility >= 6,
                    ProgressPercent = (ectsTracking.InternationalMobility / 6.0) * 100.0
                },
                new EctsCategoryBreakdown
                {
                    CategoryName = "Teaching Activities",
                    Required = 18,
                    Awarded = ectsTracking.TeachingActivities,
                    Remaining = Math.Max(0, 18 - ectsTracking.TeachingActivities),
                    IsComplete = ectsTracking.TeachingActivities >= 18,
                    ProgressPercent = (ectsTracking.TeachingActivities / 18.0) * 100.0
                },
                new EctsCategoryBreakdown
                {
                    CategoryName = "Publications",
                    Required = 27,
                    Awarded = ectsTracking.Publications,
                    Remaining = Math.Max(0, 27 - ectsTracking.Publications),
                    IsComplete = ectsTracking.Publications >= 27,
                    ProgressPercent = (ectsTracking.Publications / 27.0) * 100.0
                },
                new EctsCategoryBreakdown
                {
                    CategoryName = "Thesis Defence",
                    Required = 46,
                    Awarded = ectsTracking.ThesisDefence,
                    Remaining = Math.Max(0, 46 - ectsTracking.ThesisDefence),
                    IsComplete = ectsTracking.ThesisDefence >= 46,
                    ProgressPercent = (ectsTracking.ThesisDefence / 46.0) * 100.0
                }
            };

            _logger.LogInformation(
                "{Role} {UserId} viewed detailed ECTS breakdown for student {StudentId}",
                currentUserRole, currentUserId, request.StudentId);

            return new StudentEctsDetailedResponse
            {
                StudentId = request.StudentId,
                CurrentSemester = student.CurrentSemester,
                TotalEcts = totalEcts,
                IsCompleted = ectsTracking.IsCompleted,
                ProgressPercent = totalProgressPercent,
                Categories = categories
            };
        }
    }
}
