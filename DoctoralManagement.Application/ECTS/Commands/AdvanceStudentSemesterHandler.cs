using DoctoralManagement.Domain.Interfaces;
using MediatR;

namespace DoctoralManagement.Application.ECTS.Commands
{
    public class AdvanceStudentSemesterHandler : IRequestHandler<AdvanceStudentSemesterCommand, bool>
    {
        private readonly IStudentRepository _studentRepository;

        public AdvanceStudentSemesterHandler(IStudentRepository studentRepository)
        {
            _studentRepository = studentRepository;
        }

        public async Task<bool> Handle(AdvanceStudentSemesterCommand request, CancellationToken cancellationToken)
        {
            var student = await _studentRepository.GetByIdAsync(request.StudentId)
                ?? throw new Exception($"Student with id {request.StudentId} not found.");

            if (request.Semester < 1 || request.Semester > 6)
            {
                throw new Exception("Semester must be between 1 and 6");
            }

            student.CurrentSemester = request.Semester;
            await _studentRepository.UpdateAsync(student);

            return true;
        }
    }
}
