namespace DoctoralManagement.Application.Common
{
    public interface ICurrentUserService
    {
        int UserId { get; }
        string? Role { get; }
        string? Email { get; }
        bool CanAccessStudent(int studentId);
    }
}
