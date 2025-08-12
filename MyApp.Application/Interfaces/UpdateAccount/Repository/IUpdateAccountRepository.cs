using MyApp.Application.CQRS.UpdateAccount.Command.SendUpdateOtp;
using MyApp.Core.Entities;

namespace MyApp.Application.Interfaces.UpdateAccount.Repository
{
    public interface IUpdateAccountRepository
    {
        Task<string> GetEmailByUserIdAsync(string userId);
        Task<Account> GetAccountByUserIdAsync(string userId);
        Task<bool> IsEmailUsedByOtherAsync(Guid accountId, string email);
        Task<bool> IsPhoneUsedByOtherAsync(Guid accountId, string phone);
        Task UpdateAccountAsync(Account account);
    }
}
