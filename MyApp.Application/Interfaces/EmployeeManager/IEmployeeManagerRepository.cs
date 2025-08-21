using MyApp.Application.CQRS.EmployeeManager.ListEmployeeAccount;

namespace MyApp.Application.Interfaces.EmployeeManager
{
    public interface IEmployeeManagerRepository
    {
        Task<bool> AssignPermissionUser(Guid accountId, int roleId, Guid guid);
        Task<bool> ChangeStatusEmployeeAccount(Guid accountId, bool isActive, Guid guid);
        Task<int> GetEmployeeAccountTotal();
        Task<List<EmployeeAccountResponse>> ListEmployeeAccount(
            int pageNumber,
            int pageSize,
            string? seach,
            int? roleId
        );
    }
}
