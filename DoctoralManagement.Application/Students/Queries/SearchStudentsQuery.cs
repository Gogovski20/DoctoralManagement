using MediatR;

namespace DoctoralManagement.Application.Students.Queries
{
    public class SearchStudentsQuery : IRequest<List<SearchStudentsResponse>>
    {
        public string SearchTerm { get; set; } = string.Empty;
    }

    public class SearchStudentsResponse
    {
        public int Id { get; set; }
        public string StudentIndex { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
