using MediatR;

namespace DoctoralManagement.Application.ECTS.Queries
{
    public class GetStudentEctsDetailedQuery : IRequest<StudentEctsDetailedResponse>
    {
        public int StudentId { get; set; }
    }

    public class StudentEctsDetailedResponse
    {
        public int StudentId { get; set; }
        public int CurrentSemester { get; set; }
        public int TotalEcts { get; set; }
        public bool IsCompleted { get; set; }
        public double ProgressPercent { get; set; }

        public List<EctsCategoryBreakdown> Categories { get; set; } = new();
    }

    public class EctsCategoryBreakdown
    {
        public string CategoryName { get; set; } = string.Empty;
        public int Required { get; set; }
        public int Awarded { get; set; }
        public int Remaining { get; set; }
        public bool IsComplete { get; set; }
        public double ProgressPercent { get; set; }
    }
}
