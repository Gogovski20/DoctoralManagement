using DoctoralManagement.Domain.Interfaces;
using MediatR;

namespace DoctoralManagement.Application.ECTS.Queries
{
    public class GetStudentEctsStatusHandler : IRequestHandler<GetStudentEctsStatusQuery, StudentEctsStatusResponse>
    {
        private readonly IEctsTrackingRepository _repository;
        private readonly IStudentRepository _studentRepository;

        public GetStudentEctsStatusHandler(IEctsTrackingRepository repository, IStudentRepository studentRepository)
        {
            _repository = repository;
            _studentRepository = studentRepository;
        }

        public async Task<StudentEctsStatusResponse> Handle(GetStudentEctsStatusQuery request, CancellationToken cancellationToken)
        {
            var student = await _studentRepository.GetByIdAsync(request.StudentId)
                ?? throw new Exception($"Student with id {request.StudentId} not found.");

            var ectsTracking = await _repository.GetByStudentIdAsync(request.StudentId)
                ?? throw new Exception($"ECTS tracking record not found for student {request.StudentId}.");

            int totalEcts = ectsTracking.TotalECTS;
            double progressPercent = (totalEcts / 180.0) * 100.0;

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
