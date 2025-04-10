using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyApp.Application.CQRS.TestFilterSortPage.Queries.GetAllUser;
using MyApp.Application.CQRS.User.Queries.Authenticate;
using MyApp.Core.Entities;

namespace MyApp.Application.Interfaces
{
    public interface IUserRepository
    {
        Task<User> CheckUserLogin(LoginRequest loginRequestDTO);

        Task<List<string>> GetRoleNamesByUsername(string username);
        Task<UserResponse> GetAllUser(UserRequest userRequest);
    }
}
