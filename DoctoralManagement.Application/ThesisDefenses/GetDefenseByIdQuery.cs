using MediatR;

namespace DoctoralManagement.Application.ThesisDefenses
{
    public class GetDefenseByIdQuery : IRequest<GetDefenseByIdResponse>
    {
        public int DefenseId { get; set; }
    }

    public class GetDefenseByIdResponse
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
