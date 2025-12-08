using MediatR;

namespace DoctoralManagement.Application.Authentication.Queries
{
    public class GetMyProfileQuery : IRequest<GetMyProfileResponse>
    {
        public string UserId { get; set; } = string.Empty;
    }

    public class GetMyProfileResponse
    {
        public string UserId { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;

        public int? StudentId { get; set; }
        public int? MentorId { get; set; }

        public object? ExtraInfo { get; set; }  // optional, for student details
    }
}
