using MediatR;

namespace DoctoralManagement.Application.ThesisDefenses
{
    public class GetScheduledDefensesQuery : IRequest<IEnumerable<ScheduledDefenseResponse>>
    {
    }

    public class ScheduledDefenseResponse
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public string? StudentName { get; set; }
        public DateTime ScheduledAt { get; set; }
        public string Room { get; set; } = string.Empty;
        public int CommitteeMemberCount { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
