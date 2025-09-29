namespace DoctoralManagement.Domain.Entities
{
    public class Mobility
    {
        public int Id { get; set; }
        public string Institution { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public int StudentId { get; set; }
        // Navigation
        public Student? Student { get; set; } 
    }
}
