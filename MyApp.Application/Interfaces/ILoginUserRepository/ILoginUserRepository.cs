using MyApp.Application.CQRS.LoginUser.Queries;
using MyApp.Core.DTOs.LoginUserDTO;

namespace MyApp.Application.Interfaces.ILoginUserRepository
{
    public interface ILoginUserRepository
    {
        Task<AccountDTO> GetAccountLogin(LoginUserRequest loginRequestDTO);

        Task<string> GetRoleNameByPhoneNumber(string numberPhone);

        Task<UserDTO> GetUserByPhoneNumber(string phoneNumber);
    }
}
