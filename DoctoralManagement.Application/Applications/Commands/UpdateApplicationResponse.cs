using DoctoralManagement.Domain.Entities;

namespace DoctoralManagement.Application.Applications.Commands
{
    public class UpdateApplicationResponse
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public int DoctoralProgramId { get; set; }
        public int? PreferredMentorId { get; set; }
        public ApplicationStatus ApplicationStatus { get; set; }
        public DateTime ApplicationDate { get; set; }
    }
}
