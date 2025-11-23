using DoctoralManagement.Application.Common;
using MediatR;

namespace DoctoralManagement.Application.Authentication.Commands
{
    public class LoginHandler : IRequestHandler<LoginRequest, LoginResponse>
    {
        private readonly IAuthService _authService;
        private readonly IJwtService _jwtService;

        public LoginHandler(IJwtService jwtService, IAuthService authService)
        {
            _jwtService = jwtService;
            _authService = authService;
        }

        public async Task<LoginResponse> Handle(LoginRequest request, CancellationToken cancellationToken)
        {
            var user = await _authService.ValidateUserAsync(request.Email, request.Password);

            if (user == null) 
            {
                return new LoginResponse { Success = false, Message = "Invalid email or password." };
            }

            var token = _jwtService.GenerateTokenAsync(user);

            return new LoginResponse { Success = true, Token = token, Email = user.Email };
        }
    }
}
