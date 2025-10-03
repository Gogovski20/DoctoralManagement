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

            if (request.GPA < 8.00m)
            {
                throw new Exception("GPA must be at least 8.00 for doctoral studies!.");
            }

            if (request.TotalCreditsFromBachelor + request.TotalCreditsFromMaster < 300)
            {
                throw new Exception("Total credits from previous education must be at least 300 ECTS!.");
            }

            var student = new Student
            {
                FullName = request.FullName,
                Email = request.Email,
                IndexNumber = request.IndexNumber,
                EnrollmentDate = request.EnrollmentDate,
                GPA = request.GPA,
                EnglishCertificate = request.EnglishCertificate,
                TotalCreditsFromBachelor = request.TotalCreditsFromBachelor,
                TotalCreditsFromMaster = request.TotalCreditsFromMaster,
                //DoctoralProgramId = request.DoctoralProgramId,
                Status = StudentStatus.Active
            };
            
            var createdStudent = await _studentRepository.AddAsync(student);

            return new CreateStudentResponse
            {
                Id = createdStudent.Id,
                FullName = createdStudent.FullName,
                Email = createdStudent.Email,
                IndexNumber = createdStudent.IndexNumber,
                EnrollmentDate = createdStudent.EnrollmentDate,
                GPA = createdStudent.GPA,
                EnglishCertificate = createdStudent.EnglishCertificate,
                TotalCredits = createdStudent.TotalCredits,
                StudentStatus = createdStudent.Status
                //DoctoralProgramId = createdStudent.DoctoralProgramId,
                //DoctoralProgramName = null // This can be populated if needed
            };
        }
    }
}
