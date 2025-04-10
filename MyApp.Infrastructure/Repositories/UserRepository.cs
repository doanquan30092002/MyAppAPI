using System.Linq;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MyApp.Application.CQRS.TestFilterSortPage.Queries.GetAllUser;
using MyApp.Application.CQRS.User.Queries.Authenticate;
using MyApp.Application.Interfaces;
using MyApp.Core.Entities;
using MyApp.Infrastructure.Data;

namespace MyApp.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext context;
        private readonly IMapper mapper;

        public UserRepository(AppDbContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        public async Task<User?> CheckUserLogin(LoginRequest loginRequestDTO)
        {
            var user = await context.Users.FirstOrDefaultAsync(x =>
                x.Username.Equals(loginRequestDTO.UserName)
                && x.Password.Equals(loginRequestDTO.Password)
            );
            if (user != null)
            {
                return user;
            }
            return null;
        }

        public async Task<UserResponse> GetAllUser(UserRequest userRequest)
        {
            var users = context.Users.AsQueryable();
            // filter
            if (
                string.IsNullOrWhiteSpace(userRequest.FilterOn) == false
                && string.IsNullOrWhiteSpace(userRequest.FilterQuery) == false
            )
            {
                if (userRequest.FilterOn.Equals("Username", StringComparison.OrdinalIgnoreCase))
                {
                    users = users.Where(x => x.Username.Contains(userRequest.FilterQuery));
                }
            }
            //sort
            if (string.IsNullOrWhiteSpace(userRequest.SortBy) == false)
            {
                if (userRequest.SortBy.Equals("Id", StringComparison.OrdinalIgnoreCase))
                {
                    users = userRequest.IsAscending
                        ? users.OrderBy(x => x.Id)
                        : users.OrderByDescending(x => x.Id);
                }
            }
            // paging
            var skipResult = (userRequest.PageNumber - 1) * userRequest.PageSize;
            var result = users.Skip(skipResult).Take(userRequest.PageSize).ToListAsync();
            var resultDTO = mapper.Map<UserResponse>(result);
            return resultDTO;
        }

        public async Task<List<string>> GetRoleNamesByUsername(string username)
        {
            var roleNames = await context
                .UserRoles.Where(ur => ur.User.Username == username)
                .Select(ur => ur.Role.RoleName)
                .ToListAsync();

            return roleNames;
        }
    }
}
