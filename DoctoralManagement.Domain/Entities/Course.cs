namespace DoctoralManagement.Domain.Entities
{
    public class Course
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty; // e.g., "CS501"
        public string Title { get; set; } = string.Empty; // e.g., "Machine Learning"
        public int EctsCredits { get; set; } // e.g., 6
        public string InstructorName { get; set; } = string.Empty;
        public int Semester { get; set; } // 1-6
        public string Description { get; set; } = string.Empty;

        // Navigation
        public ICollection<CourseEnrollment> Enrollments { get; set; } = new List<CourseEnrollment>();
    }
}
