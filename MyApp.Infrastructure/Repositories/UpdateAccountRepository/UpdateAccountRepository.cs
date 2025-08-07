using Microsoft.EntityFrameworkCore;
using MyApp.Application.Interfaces.UpdateAccount.Repository;
using MyApp.Core.Entities;
using MyApp.Infrastructure.Data;

namespace MyApp.Infrastructure.Repositories.UpdateAccountRepository
{
    public class UpdateAccountRepository : IUpdateAccountRepository
    {
        private readonly AppDbContext _context;

        public UpdateAccountRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<string> GetEmailByUserIdAsync(string? userId)
        {
            if (string.IsNullOrEmpty(userId))
                return null;

            var account = await _context
                .Accounts.Include(x => x.User)
                .FirstOrDefaultAsync(x => x.User.Id.ToString() == userId);

            return account?.Email;
        }

        public async Task<Account> GetAccountByUserIdAsync(string userId)
        {
            return await _context
                .Accounts.Include(x => x.User)
                .FirstOrDefaultAsync(x => x.User.Id.ToString() == userId);
        }

        public async Task<bool> IsEmailUsedByOtherAsync(Guid accountId, string email)
        {
            if (string.IsNullOrEmpty(email))
                return false;

            return await _context.Accounts.AnyAsync(x =>
                x.Email == email && x.AccountId != accountId
            );
        }

        public async Task<bool> IsPhoneUsedByOtherAsync(Guid accountId, string phone)
        {
            if (string.IsNullOrEmpty(phone))
                return false;

            return await _context.Accounts.AnyAsync(x =>
                x.PhoneNumber == phone && x.AccountId != accountId
            );
        }

        public async Task UpdateAccountAsync(Account account)
        {
            _context.Accounts.Update(account);
            await _context.SaveChangesAsync();
        }
    }
}
