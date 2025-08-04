using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Http;
using MyApp.Application.Interfaces.EmployeeManager;

namespace MyApp.Application.CQRS.EmployeeManager.ChangeStatusEmployeeAccount
{
    public class ChangeStatusEmployeeAccountHandler
        : IRequestHandler<ChangeStatusEmployeeAccountRequest, bool>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IEmployeeManagerRepository _employeeManagerRepository;

        public ChangeStatusEmployeeAccountHandler(
            IEmployeeManagerRepository employeeManagerRepository,
            IHttpContextAccessor httpContextAccessor
        )
        {
            _employeeManagerRepository = employeeManagerRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<bool> Handle(
            ChangeStatusEmployeeAccountRequest request,
            CancellationToken cancellationToken
        )
        {
            var userIdStr = _httpContextAccessor
                .HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)
                ?.Value;
            bool changeStatusEmployeeAccountResponse =
                await _employeeManagerRepository.ChangeStatusEmployeeAccount(
                    request.AccountId,
                    request.IsActive,
                    Guid.Parse(userIdStr)
                );
            return changeStatusEmployeeAccountResponse;
        }
    }
}
