using MediatR;
using MyApp.Application.Interfaces.EmployeeManager;

namespace MyApp.Application.CQRS.EmployeeManager.ListEmployeeAccount
{
    public class ListEmployeeAccountHandler
        : IRequestHandler<ListEmployeeAccountRequest, ListEmployeeAccountResponse>
    {
        private readonly IEmployeeManagerRepository _employeeManagerRepository;

        public ListEmployeeAccountHandler(IEmployeeManagerRepository employeeManagerRepository)
        {
            _employeeManagerRepository = employeeManagerRepository;
        }

        public async Task<ListEmployeeAccountResponse> Handle(
            ListEmployeeAccountRequest request,
            CancellationToken cancellationToken
        )
        {
            List<EmployeeAccountResponse> employeeAccounts =
                await _employeeManagerRepository.ListEmployeeAccount(
                    request.PageNumber,
                    request.PageSize,
                    request.Seach,
                    request.RoleId
                );
            return new ListEmployeeAccountResponse
            {
                EmployeeAccounts = employeeAccounts,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalCount = employeeAccounts.Count,
            };
        }
    }
}
