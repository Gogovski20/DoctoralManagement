using DoctoralManagement.Application.Common;
using MediatR;

namespace DoctoralManagement.Application.Authentication.Queries
{
    public class GetAllUsersHandler : IRequestHandler<GetAllUsersQuery, IEnumerable<GetAllUsersResponse>>
    {
        private readonly IUserService _userService;

        public GetAllUsersHandler(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<IEnumerable<GetAllUsersResponse>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
        {
            var users = await _userService.GetAllUsers();

            return users.Select(user => new GetAllUsersResponse
            {
                UserId = user.Id,
                UserName = user.FullName,
                Email = user.Email,
                Role = user.Role
            });
        }
    }
}
