namespace DoctoralManagement.Application.ThesisDefenses
{
    public class ScheduleThesisDefenseResponse
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public DateTime ScheduledAt { get; set; }
        public string Room { get; set; } = string.Empty;
        public List<int> CommitteeMemberIds { get; set; } = new();
        public string Status { get; set; } = string.Empty;
    }
}
