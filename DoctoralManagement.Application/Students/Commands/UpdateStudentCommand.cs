using DoctoralManagement.Domain.Entities;
using MediatR;

namespace DoctoralManagement.Application.Students.Commands
{
    public class UpdateStudentCommand : IRequest<UpdateStudentResponse>
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public decimal GPA { get; set; }
        public string EnglishCertificate { get; set; } = string.Empty;
        public StudentStatus StudentStatus { get; set; }
        //public int? DoctoralProgramId { get; set; }
    }
}
