namespace DoctoralManagement.Domain.Entities
{
    public class Publication
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Journal { get; set; } = string.Empty;
        public DateTime PublishedOn { get; set; }

        public int StudentId { get; set; }
        // Navigation
        public Student? Student { get; set; }
    }
}
