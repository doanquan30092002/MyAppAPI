using MediatR;
using MyApp.Application.CQRS.LoginUser.Queries;
using MyApp.Core.DTOs.LoginUserDTO;

namespace MyApp.Application.Interfaces.ILoginUserRepository
{
    public interface ILoginUserRepository
    {
        Task<AccountDTO> GetAccountLogin(string email, string password);

        Task<string> GetRoleNameByEmail(string email);

        Task<UserDTO> GetUserByEmail(string email);
    }
}
