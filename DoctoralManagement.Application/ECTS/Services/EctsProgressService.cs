using DoctoralManagement.Domain.Interfaces;

namespace DoctoralManagement.Application.ECTS.Services
{
    public class EctsProgressService
    {
        private readonly IStudentRepository _studentRepository;

        public EctsProgressService(IStudentRepository studentRepository)
        {
            _studentRepository = studentRepository;
        }

        public async Task UpdateStudentSemesterAsync(int studentId, int totalEcts)
        {
            var student = await _studentRepository.GetByIdAsync(studentId);
            if (student == null) 
            {
                throw new Exception($"Student with id {studentId} not found.");
            }

            int newSemester = Math.Min((totalEcts / 30) + 1, 6);

            if (student.CurrentSemester != newSemester)
            {
                student.CurrentSemester = newSemester;
                await _studentRepository.UpdateAsync(student);
            }
        }
    }
}
