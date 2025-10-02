namespace DoctoralManagement.Domain.Entities
{
    public class ECTSTracking
    {
        public int Id { get; set; }
        public int StudentId { get; set; }

        // UKIM ECTS Breakdown
        public int OrganizedAcademicTraining { get; set; } // 42 ECTS
        public int IndependentResearchProject { get; set; } // 41 ECTS
        public int InternationalMobility { get; set; } // 6 ECTS
        public int TeachingActivities { get; set; } // 18 ECTS
        public int Publications { get; set; } // 27 ECTS
        public int ThesisDefence { get; set; } // 46 ECTS

        public int TotalECTS => OrganizedAcademicTraining + IndependentResearchProject + 
            InternationalMobility + TeachingActivities + Publications + ThesisDefence;

        public bool IsCompleted => TotalECTS >= 180;

        // Navigation
        public Student Student { get; set; } = null!;
    }
}
