using DoctoralManagement.Domain.Interfaces;
using MediatR;

namespace DoctoralManagement.Application.Students.Commands
{
    public class UpdateStudentHandler : IRequestHandler<UpdateStudentCommand, UpdateStudentResponse>
    {
        private readonly IStudentRepository _studentRepository;

        public UpdateStudentHandler(IStudentRepository studentRepository)
        {
            _studentRepository = studentRepository;
        }

        public async Task<UpdateStudentResponse> Handle(UpdateStudentCommand request, CancellationToken cancellationToken)
        {
            var student = await _studentRepository.GetByIdAsync(request.Id);

            if (student == null)
            {
                throw new Exception($"Student with id: {request.Id} not found.");
            }

            if (student.Email != request.Email && await _studentRepository.ExistsByEmailAsync(request.Email))
            {
                throw new Exception($"Student with email: {request.Email} already exists.");
            }

            student.FullName = request.FullName;
            student.Email = request.Email;
            student.GPA = request.GPA;
            student.EnglishCertificate = request.EnglishCertificate;
            student.Status = request.StudentStatus;

            await _studentRepository.UpdateAsync(student);

            return new UpdateStudentResponse
            {
                Id = student.Id,
                FullName = student.FullName,
                Email = student.Email,
                IndexNumber = student.IndexNumber,
                EnrollmentDate = student.EnrollmentDate,
                GPA = student.GPA,
                EnglishCertificate = student.EnglishCertificate,
                StudentStatus = student.Status,
                TotalCredits = student.TotalCredits
            };
        }
    }
}
