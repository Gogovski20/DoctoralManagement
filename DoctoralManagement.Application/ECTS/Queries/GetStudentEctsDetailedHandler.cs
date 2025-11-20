using DoctoralManagement.Domain.Interfaces;
using MediatR;

namespace DoctoralManagement.Application.ECTS.Queries
{
    public class GetStudentEctsDetailedHandler : IRequestHandler<GetStudentEctsDetailedQuery, StudentEctsDetailedResponse>
    {
        private readonly IEctsTrackingRepository _ectsTrackingRepository;
        private readonly IStudentRepository _studentRepository;

        public GetStudentEctsDetailedHandler(IEctsTrackingRepository ectsTrackingRepository, IStudentRepository studentRepository)
        {
            _ectsTrackingRepository = ectsTrackingRepository;
            _studentRepository = studentRepository;
        }

        public async Task<StudentEctsDetailedResponse> Handle(GetStudentEctsDetailedQuery request, CancellationToken cancellationToken)
        {
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
