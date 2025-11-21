using MediatR;

namespace DoctoralManagement.Application.ECTS.Commands
{
    public class AdvanceStudentSemesterCommand : IRequest<bool>
    {
        public int StudentId { get; set; }
        public int Semester { get; set; }
    }
}
