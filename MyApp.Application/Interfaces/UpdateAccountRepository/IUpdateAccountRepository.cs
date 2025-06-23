using MyApp.Application.CQRS.UpdateAccountAndProfile.Command;

namespace MyApp.Application.Interfaces.UpdateAccountRepository
{
    public interface IUpdateAccountRepository
    {
        Task<UpdateAccountResponse> UpdateAccountInfo(
            string? userId,
            string? emailNew,
            string? passwordOld,
            string? passwordNew,
            string? phoneNumberNew
        );
    }
}
