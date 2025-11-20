using MediatR;

namespace DoctoralManagement.Application.Courses.Commands
{
    public class CompleteCourseEnrollmentCommand : IRequest<bool>
    {
        public int StudentId { get; set; }
        public int EnrollmentId { get; set; }
        public decimal Grade { get; set; }
    }
}
