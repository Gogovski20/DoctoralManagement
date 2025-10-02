using DoctoralManagement.Domain.Interfaces;
using MediatR;

namespace DoctoralManagement.Application.Students.Queries
{
    public class GetStudentByIdHandler : IRequestHandler<GetStudentByIdQuery, GetStudentByIdResponse>
    {
        private readonly IStudentRepository _studentRepository;

        public GetStudentByIdHandler(IStudentRepository studentRepository)
        {
            _studentRepository = studentRepository;
        }

        public async Task<GetStudentByIdResponse> Handle(GetStudentByIdQuery request, CancellationToken cancellationToken)
        {
            var student = await _studentRepository.GetByIdAsync(request.Id);
            
            if (student == null)
            {
                throw new KeyNotFoundException($"Student with ID {request.Id} not found.");
            }
            
            return new GetStudentByIdResponse
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
