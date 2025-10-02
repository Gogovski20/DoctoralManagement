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

            student.FullName = request.FullName;
            student.Email = request.Email;

            await _studentRepository.UpdateAsync(student);

            return new UpdateStudentResponse
            {
                Id = student.Id,
                FullName = student.FullName,
                Email = student.Email,
                IndexNumber = student.IndexNumber,
                EnrollmentDate = student.EnrollmentDate,
                TotalCredits = student.TotalCredits
            };
        }
    }
}
