using DoctoralManagement.Domain.Entities;
using MediatR;

namespace DoctoralManagement.Application.ThesisDefenses
{
    public class GetAllThesisDefensesQuery : IRequest<IEnumerable<GetAllThesisDefensesResponse>>
    {
    }

    public class GetAllThesisDefensesResponse
    {
        public int Id { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public string ProjectTitle { get; set; } = string.Empty;
        public DateTime ScheduledAt { get; set; }
        public string Room { get; set; } = string.Empty;
        public List<int> CommitteeMembers { get; set; } = new List<int>();
        public DefenseStatus Status { get; set; }
        public string? ResultNotes { get; set; }
        public DateTime? CompletedAt { get; set; }
        public string? ArchiveNumber { get; set; }
        public List<CommitteeReview> Reviews { get; set; } = new List<CommitteeReview>();
    }
}
