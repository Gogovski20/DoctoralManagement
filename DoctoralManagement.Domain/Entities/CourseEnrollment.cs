namespace DoctoralManagement.Domain.Entities
{
    public class CourseEnrollment
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public int CourseId { get; set; }
        public DateTime EnrolledDate { get; set; }
        public bool Completed { get; set; } = false;
        public decimal? Grade { get; set; } 

        // Navigation
        public Student Student { get; set; } = null!;
        public Course Course { get; set; } = null!;
    }
}
