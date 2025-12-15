using MediatR;
using DoctoralManagement.Domain.Interfaces;

namespace DoctoralManagement.Application.Students.Queries
{
    public class SearchStudentsHandler : IRequestHandler<SearchStudentsQuery, List<SearchStudentsResponse>>
    {
        private readonly IStudentRepository _studentRepository;

        public SearchStudentsHandler(IStudentRepository studentRepository)
        {
            _studentRepository = studentRepository;
        }

        public async Task<List<SearchStudentsResponse>> Handle(SearchStudentsQuery request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                return new List<SearchStudentsResponse>();
            }

            // Call the repository method (does all the work in the database)
            var searchResults = await _studentRepository.SearchStudentsAsync(request.SearchTerm);

            // Transform to response DTO
            return searchResults
                .Take(20) // Just in case, limit to 20
                .Select(s => new SearchStudentsResponse
                {
                    Id = s.Id,
                    StudentIndex = s.StudentIndex,
                    FullName = s.FullName,
                    Email = s.Email,
                    CreatedAt = s.CreatedAt
                })
                .ToList();
        }
    }
}
