namespace DoctoralManagement.Domain.Entities
{
    public class Course
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public int EctsCredits { get; set; } 
        public string InstructorName { get; set; } = string.Empty;
        public int Semester { get; set; } 
        public string Description { get; set; } = string.Empty;

        // Navigation
        public ICollection<CourseEnrollment> Enrollments { get; set; } = new List<CourseEnrollment>();
    }
}
