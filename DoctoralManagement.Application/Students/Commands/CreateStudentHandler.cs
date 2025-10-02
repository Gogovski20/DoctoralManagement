using DoctoralManagement.Domain.Entities;
using DoctoralManagement.Domain.Interfaces;
using MediatR;

namespace DoctoralManagement.Application.Students.Commands
{
    public class CreateStudentHandler : IRequestHandler<CreateStudentCommand, CreateStudentResponse>
    {
        private readonly IStudentRepository _studentRepository;

        public CreateStudentHandler(IStudentRepository studentRepository)
        {
            _studentRepository = studentRepository;
        }

        public async Task<CreateStudentResponse> Handle(CreateStudentCommand request, CancellationToken cancellationToken)
        {
            if (await _studentRepository.ExistsByEmailAsync(request.Email))
            {
                throw new Exception("A student with the same email already exists.");
            }

            if (await _studentRepository.ExistsByIndexNumberAsync(request.IndexNumber))
            {
                throw new Exception("A student with the same index number already exists.");
            }

            var student = new Student
            {
                FullName = request.FullName,
                Email = request.Email,
                IndexNumber = request.IndexNumber,
                EnrollmentDate = request.EnrollmentDate,
                TotalCreditsFromBachelor = request.TotalCreditsFromBachelor,
                TotalCreditsFromMaster = request.TotalCreditsFromMaster
            };
            
            var createdStudent = await _studentRepository.AddAsync(student);

            return new CreateStudentResponse
            {
                Id = createdStudent.Id,
                FullName = createdStudent.FullName,
                Email = createdStudent.Email,
                IndexNumber = createdStudent.IndexNumber,
                EnrollmentDate = createdStudent.EnrollmentDate,
                TotalCredits = createdStudent.TotalCredits
            };
        }
    }
}
