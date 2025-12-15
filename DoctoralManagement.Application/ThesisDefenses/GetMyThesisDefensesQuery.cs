using MediatR;

namespace DoctoralManagement.Application.ThesisDefenses
{
    public class GetMyThesisDefensesQuery : IRequest<IEnumerable<GetMyThesisDefensesResponse>>
    {
        public int StudentId { get; set; }
    }

    public class GetMyThesisDefensesResponse
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public string ProjectTitle { get; set; } = string.Empty;
        public DateTime ScheduledAt { get; set; }
        public string Room { get; set; } = string.Empty;
        public List<int> CommitteeMembers { get; set; } = new List<int>();
        public string Status { get; set; } = string.Empty;
    }
}
