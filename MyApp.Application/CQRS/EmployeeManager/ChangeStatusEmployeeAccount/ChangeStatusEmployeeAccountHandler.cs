using MediatR;
using MyApp.Application.Common.CurrentUserService;
using MyApp.Application.Interfaces.EmployeeManager;

namespace MyApp.Application.CQRS.EmployeeManager.ChangeStatusEmployeeAccount
{
    public class ChangeStatusEmployeeAccountHandler
        : IRequestHandler<ChangeStatusEmployeeAccountRequest, bool>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IEmployeeManagerRepository _employeeManagerRepository;

        public ChangeStatusEmployeeAccountHandler(
            IEmployeeManagerRepository employeeManagerRepository,
            ICurrentUserService currentUserService
        )
        {
            _employeeManagerRepository = employeeManagerRepository;
            _currentUserService = currentUserService;
        }

        public async Task<bool> Handle(
            ChangeStatusEmployeeAccountRequest request,
            CancellationToken cancellationToken
        )
        {
            var userId = _currentUserService.GetUserId();
            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedAccessException("Yêu cầu đăng nhập");
            bool changeStatusEmployeeAccountResponse =
                await _employeeManagerRepository.ChangeStatusEmployeeAccount(
                    request.AccountId,
                    request.IsActive,
                    Guid.Parse(userId)
                );
            return changeStatusEmployeeAccountResponse;
        }
    }
}
