namespace DoctoralManagement.Application.ThesisDefenses
{
    public class CompleteThesisDefenseResponse
    {
        public int DefenseId { get; set; }
        public int ProjectId { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? ArchiveNumber { get; set; }
        public DateTime? CompletedAt { get; set; }
    }
}
