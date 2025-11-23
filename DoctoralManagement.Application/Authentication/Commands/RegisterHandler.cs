using DoctoralManagement.Application.Common;
using DoctoralManagement.Application.Dtos;
using MediatR;

namespace DoctoralManagement.Application.Authentication.Commands
{
    public class RegisterHandler : IRequestHandler<RegisterRequest, RegisterResponse>
    {
        private readonly IAuthService _authService;

        public RegisterHandler(IAuthService authService)
        {
            _authService = authService;
        }

        public async Task<RegisterResponse> Handle(RegisterRequest request, CancellationToken cancellationToken)
        {
            if (await _authService.UserExistsAsync(request.Email))
            {
                return new RegisterResponse { Success = false, Message = "This email is already registered." };
            }

            await _authService.CreateUserAsync(
                new RegisterUserDto(request.FullName, request.Email, request.Role),
                request.Password
            );

            return new RegisterResponse { Success = true, Message = "User registered successfully." };
        }
    }
}
