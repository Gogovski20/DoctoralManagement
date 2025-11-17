using MediatR;

namespace DoctoralManagement.Application.ThesisDefenses
{
    public class ScheduleThesisDefenseCommand : IRequest<ScheduleThesisDefenseResponse>
    {
        public int ProjectId { get; set; }
        public DateTime ScheduledAt { get; set; }
        public string Room { get; set; } = string.Empty;
        public List<int> CommitteeMemberIds { get; set; } = new();
    }
}
