using MediatR;

namespace DoctoralManagement.Application.Students.Commands
{
    public class CreateStudentCommand : IRequest<CreateStudentResponse>
    {
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string IndexNumber { get; set; } = string.Empty;
        public DateTime EnrollmentDate { get; set; }
        public int TotalCreditsFromBachelor { get; set; }
        public int TotalCreditsFromMaster { get; set; }
    }
}
