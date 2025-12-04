using DoctoralManagement.Application.Common;
using DoctoralManagement.Domain.Exceptions;
using MediatR;
using System.Net;

namespace DoctoralManagement.Application.Authentication.Queries
{
    public class GetUserByIdHandler : IRequestHandler<GetUserByIdQuery, GetUserByIdResponse>
    {
        private readonly IUserService _userService;

        public GetUserByIdHandler(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<GetUserByIdResponse> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        {
            var user = await _userService.GetUserById(request.UserId)
                ?? throw new DoctoralManagementException("User not found.", HttpStatusCode.NotFound);

            return new GetUserByIdResponse
            {
                UserId = user.Id,
                UserName = user.FullName,
                UserEmail = user.Email,
                Role = user.Role,
            };
        }
    }
}
