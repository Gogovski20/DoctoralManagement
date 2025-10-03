using DoctoralManagement.Domain.Interfaces;
using MediatR;

namespace DoctoralManagement.Application.Students.Queries
{
    public class GetAllStudentsHandler : IRequestHandler<GetAllStudentsQuery, IEnumerable<GetAllStudentsResponse>>
    {
        private readonly IStudentRepository _studentRepository;

        public GetAllStudentsHandler(IStudentRepository studentRepository)
        {
            _studentRepository = studentRepository;
        }

        public async Task<IEnumerable<GetAllStudentsResponse>> Handle(GetAllStudentsQuery request, CancellationToken cancellationToken)
        {
            var students = await _studentRepository.GetAllAsync();

            return students.Select(s => new GetAllStudentsResponse
            {
                Id = s.Id,
                FullName = s.FullName,
                Email = s.Email,
                IndexNumber = s.IndexNumber,
                GPA = s.GPA,
                StudentStatus = s.Status,
                TotalCredits = s.TotalCredits
            });
        }
    }
}
