using DoctoralManagement.Domain.Interfaces;
using MediatR;

namespace DoctoralManagement.Application.Students.Commands
{
    public class DeleteStudentHandler : IRequestHandler<DeleteStudentCommand, DeleteStudentResponse>
    {
        private readonly IStudentRepository _studentRepository;

        public DeleteStudentHandler(IStudentRepository studentRepository)
        {
            _studentRepository = studentRepository;
        }

        public async Task<DeleteStudentResponse> Handle(DeleteStudentCommand request, CancellationToken cancellationToken)
        {
            var student = await _studentRepository.GetByIdAsync(request.Id);

            if (student == null)
            {
                throw new KeyNotFoundException($"Student with ID {request.Id} not found.");
            }

            await _studentRepository.DeleteAsync(student);
            
            return new DeleteStudentResponse();
        }
    }
}
