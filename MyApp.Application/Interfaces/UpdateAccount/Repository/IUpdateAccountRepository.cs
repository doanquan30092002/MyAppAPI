using MyApp.Application.CQRS.UpdateAccount.Command.SendUpdateOtp;

namespace MyApp.Application.Interfaces.UpdateAccount.Repository
{
    public interface IUpdateAccountRepository
    {
        Task<string> GetEmailByUserIdAsync(string? userId);
        Task<UpdateAccountResponse> UpdateAccountInfo(
            string? userId,
            string? emailNew,
            string? passwordOld,
            string? passwordNew,
            string? phoneNumberNew
        );
    }
}
