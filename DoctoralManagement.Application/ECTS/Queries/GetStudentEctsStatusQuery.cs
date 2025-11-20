using MediatR;

namespace DoctoralManagement.Application.ECTS.Queries
{
    public class GetStudentEctsStatusQuery : IRequest<StudentEctsStatusResponse>
    {
        public int StudentId { get; set; }
    }

    public class StudentEctsStatusResponse
    {
        public int StudentId { get; set; }
        public int OrganizedAcademicTraining { get; set; }
        public int IndependentResearchProject { get; set; }
        public int InternationalMobility { get; set; }
        public int TeachingActivities { get; set; }
        public int Publications { get; set; }
        public int ThesisDefence { get; set; }
        public int TotalEcts { get; set; }
        public int CurrentSemester { get; set; }
        public bool IsCompleted { get; set; }
        public double ProgressPercent { get; set; }
    }
}
