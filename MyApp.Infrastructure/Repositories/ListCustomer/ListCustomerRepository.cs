using Microsoft.EntityFrameworkCore;
using MyApp.Application.Common.Utils;
using MyApp.Application.CQRS.ListCustomer;
using MyApp.Application.Interfaces.ListCustomer;
using MyApp.Infrastructure.Data;

namespace MyApp.Infrastructure.Repositories.ListCustomer
{
    internal class ListCustomerRepository : IListCustomerRepository
    {
        private readonly AppDbContext _context;

        public ListCustomerRepository(AppDbContext context)
        {
            _context = context;
        }

        public Task<List<CustomerInfo>?> ListCustomer(int pageNumber, int pageSize, string? search)
        {
            var query = _context
                .Accounts.Include(x => x.User)
                .Include(x => x.Role)
                .Where(x => x.RoleId == 2);
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
                .AsEnumerable();
            if (!string.IsNullOrEmpty(search))
            {
                var keyword = StringHelper.RemoveDiacritics(search).ToLower();
                projected = projected.Where(x =>
                    x.Email.Contains(search, StringComparison.OrdinalIgnoreCase)
                    || StringHelper.RemoveDiacritics(x.Name).ToLower().Contains(keyword)
                    || x.CitizenIdentification.Contains(search)
                    || x.PhoneNumber.Contains(search)
                );
            }
            var result = projected
                .OrderByDescending(x => x.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new CustomerInfo
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
