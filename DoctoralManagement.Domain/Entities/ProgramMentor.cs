namespace DoctoralManagement.Domain.Entities
{
    public class ProgramMentor
    {
        public int DoctoralProgramId { get; set; }
        public int MentorId { get; set; }
        public string Role { get; set; } = "Member"; // Possible values: "Member", "Coordinator"
        public bool IsActive { get; set; } = true;

        // Navigation
        public DoctoralProgram DoctoralProgram { get; set; } = null!;
        public Mentor Mentor { get; set; } = null!;
    }
}
