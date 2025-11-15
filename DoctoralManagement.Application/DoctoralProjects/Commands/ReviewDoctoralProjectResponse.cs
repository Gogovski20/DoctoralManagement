namespace DoctoralManagement.Application.DoctoralProjects.Commands
{
    public class ReviewDoctoralProjectResponse
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string? CommitteeNotes { get; set; }
        public DateTime? DecisionAt { get; set; }
    }
}
