using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Http;
using MyApp.Application.Common.CurrentUserService;
using MyApp.Application.Interfaces.EmployeeManager;

namespace MyApp.Application.CQRS.EmployeeManager.AssignPermissionUser
{
    public class AssignPermissionUserHandler : IRequestHandler<AssignPermissionUserRequest, bool>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IEmployeeManagerRepository _employeeManagerRepository;

        public AssignPermissionUserHandler(
            IEmployeeManagerRepository employeeManagerRepository,
            ICurrentUserService currentUserService
        )
        {
            _employeeManagerRepository = employeeManagerRepository;
            _currentUserService = currentUserService;
        }

        public async Task<bool> Handle(
            AssignPermissionUserRequest request,
            CancellationToken cancellationToken
        )
        {
            var userId = _currentUserService.GetUserId();
            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedAccessException("Yêu cầu đăng nhập");
            bool response = await _employeeManagerRepository.AssignPermissionUser(
                request.AccountId,
                request.RoleId,
                Guid.Parse(userId)
            );
            return response;
        }
    }
}
