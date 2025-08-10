using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Http;
using MyApp.Application.Interfaces.EmployeeManager;

namespace MyApp.Application.CQRS.EmployeeManager.AssignPermissionUser
{
    public class AssignPermissionUserHandler : IRequestHandler<AssignPermissionUserRequest, bool>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IEmployeeManagerRepository _employeeManagerRepository;

        public AssignPermissionUserHandler(
            IEmployeeManagerRepository employeeManagerRepository,
            IHttpContextAccessor httpContextAccessor
        )
        {
            _employeeManagerRepository = employeeManagerRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<bool> Handle(
            AssignPermissionUserRequest request,
            CancellationToken cancellationToken
        )
        {
            var userIdStr = _httpContextAccessor
                .HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)
                ?.Value;
            if (string.IsNullOrEmpty(userIdStr))
                throw new UnauthorizedAccessException("Yêu cầu đăng nhập");
            bool response = await _employeeManagerRepository.AssignPermissionUser(
                request.AccountId,
                request.RoleId,
                Guid.Parse(userIdStr)
            );
            return response;
        }
    }
}
