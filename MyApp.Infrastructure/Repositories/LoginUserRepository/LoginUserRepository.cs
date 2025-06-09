using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MyApp.Application.CQRS.LoginUser.Queries;
using MyApp.Application.Interfaces.ILoginUserRepository;
using MyApp.Core.DTOs.LoginUserDTO;
using MyApp.Infrastructure.Data;

namespace MyApp.Infrastructure.Repositories.LoginUserRepository
{
    public class LoginUserRepository : ILoginUserRepository
    {
        private readonly AppDbContext context;
        private readonly IMapper mapper;

        public LoginUserRepository(AppDbContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        public async Task<AccountDTO> GetAccountLogin(LoginUserRequest loginRequestDTO)
        {
            var account = await context.Accounts.FirstOrDefaultAsync(x =>
                x.PhoneNumber.Equals(loginRequestDTO.PhoneNumber)
                && x.Password.Equals(loginRequestDTO.Password)
            );
            if (account != null)
            {
                return mapper.Map<AccountDTO>(account);
            }
            return null;
        }

        public async Task<string> GetRoleNameByPhoneNumber(string phoneNumber)
        {
            var roleNames = await context
                .Accounts.Include(x => x.Role)
                .Where(x => x.PhoneNumber == phoneNumber)
                .Select(ur => ur.Role.RoleName)
                .FirstOrDefaultAsync();

            return roleNames;
        }

        public async Task<UserDTO> GetUserByPhoneNumber(string phoneNumber)
        {
            var user = await context
                .Accounts.Include(x => x.User)
                .FirstOrDefaultAsync(x => x.PhoneNumber == phoneNumber);
            return user != null ? mapper.Map<UserDTO>(user.User) : null;
        }
    }
}
