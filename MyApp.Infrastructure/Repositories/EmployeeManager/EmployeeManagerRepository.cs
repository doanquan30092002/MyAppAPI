using Microsoft.EntityFrameworkCore;
using MyApp.Application.Common.Utils;
using MyApp.Application.CQRS.EmployeeManager.ListEmployeeAccount;
using MyApp.Application.Interfaces.EmployeeManager;
using MyApp.Infrastructure.Data;

namespace MyApp.Infrastructure.Repositories.EmployeeManager
{
    public class EmployeeManagerRepository : IEmployeeManagerRepository
    {
        private readonly AppDbContext context;

        public EmployeeManagerRepository(AppDbContext context)
        {
            this.context = context;
        }

        public async Task<bool> AssignPermissionUser(Guid accountId, int roleId, Guid userId)
        {
            var account = await context.Accounts.FirstOrDefaultAsync(x => x.AccountId == accountId);
            if (account == null)
            {
                return false; // Account not found
            }
            account.RoleId = roleId;
            account.UpdatedAt = DateTime.UtcNow;
            account.UpdatedBy = userId;
            context.Accounts.Update(account);
            try
            {
                await context.SaveChangesAsync();
                return true; // Successfully updated
            }
            catch (Exception)
            {
                return false; // Handle any exceptions that may occur during save
            }
        }

        public async Task<bool> ChangeStatusEmployeeAccount(
            Guid accountId,
            bool isActive,
            Guid userId
        )
        {
            var account = await context.Accounts.FirstOrDefaultAsync(x => x.AccountId == accountId);
            if (account == null)
            {
                return false; // Account not found
            }
            account.IsActive = isActive;
            account.UpdatedAt = DateTime.UtcNow;
            account.UpdatedBy = userId;
            context.Accounts.Update(account);
            try
            {
                await context.SaveChangesAsync();
                return true; // Successfully updated
            }
            catch (Exception)
            {
                return false; // Handle any exceptions that may occur during save
            }
        }

        public Task<int> GetEmployeeAccountTotal()
        {
            return context.Accounts.CountAsync(x => x.RoleId != 1 && x.RoleId != 2); // Exclude Admin, Customer
        }

        public Task<List<EmployeeAccountResponse>> ListEmployeeAccount(
            int pageNumber,
            int pageSize,
            string? seach,
            int? roleId
        )
        {
            var query = context
                .Accounts.Include(x => x.User)
                .Include(x => x.Role)
                .Where(x => x.RoleId != 1 && x.RoleId != 2); // Exclude Admin, Customer

            if (roleId != null)
            {
                query = query.Where(x => x.RoleId == roleId);
            }

            var projected = query
                .Select(x => new
                {
                    x.AccountId,
                    x.PhoneNumber,
                    x.Email,
                    x.IsActive,
                    x.RoleId,
                    RoleName = x.Role.RoleName,
                    x.UserId,
                    x.User.CitizenIdentification,
                    Name = x.User.Name,
                    x.User.BirthDay,
                    x.User.Nationality,
                    x.User.Gender,
                    x.User.ValidDate,
                    x.User.OriginLocation,
                    x.User.RecentLocation,
                    x.User.IssueDate,
                    x.User.IssueBy,
                    x.CreatedAt,
                    x.CreatedBy,
                    x.UpdatedAt,
                    x.UpdatedBy,
                })
                .AsEnumerable(); // <-- Chuyển sang C# để xử lý RemoveDiacritics

            if (!string.IsNullOrEmpty(seach))
            {
                var keyword = StringHelper.RemoveDiacritics(seach).ToLower();
                projected = projected.Where(x =>
                    x.Email.Contains(seach, StringComparison.OrdinalIgnoreCase)
                    || StringHelper.RemoveDiacritics(x.Name).ToLower().Contains(keyword)
                );
            }

            var result = projected
                .OrderByDescending(x => x.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new EmployeeAccountResponse
                {
                    AccountId = x.AccountId,
                    PhoneNumber = x.PhoneNumber,
                    Email = x.Email,
                    IsActive = x.IsActive,
                    RoleId = x.RoleId,
                    RoleName = x.RoleName,
                    UserId = x.UserId,
                    CitizenIdentification = x.CitizenIdentification,
                    Name = x.Name,
                    BirthDay = x.BirthDay,
                    Nationality = x.Nationality,
                    Gender = x.Gender,
                    ValidDate = x.ValidDate,
                    OriginLocation = x.OriginLocation,
                    RecentLocation = x.RecentLocation,
                    IssueDate = x.IssueDate,
                    IssueBy = x.IssueBy,
                    CreatedAt = x.CreatedAt,
                    CreatedBy = x.CreatedBy,
                    UpdatedAt = x.UpdatedAt,
                    UpdatedBy = x.UpdatedBy,
                })
                .ToList();

            return Task.FromResult(result);
        }
    }
}
