namespace DoctoralManagement.Application.Authentication.Commands
{
    public class LoginResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? Token { get; set; }
        public string Email { get; set; } = string.Empty;
    }
}
