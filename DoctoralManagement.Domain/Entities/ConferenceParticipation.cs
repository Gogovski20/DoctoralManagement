namespace DoctoralManagement.Domain.Entities
{
    public class ConferenceParticipation
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public Student? Student { get; set; }

        public string ConferenceName { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public string Role { get; set; } = "Participant";
        public int EctsAwarded { get; set; } = 0;
        public string? EvidencePath { get; set; }
    }
}
