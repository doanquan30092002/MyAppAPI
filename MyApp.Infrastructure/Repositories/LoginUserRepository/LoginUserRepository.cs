using AutoMapper;
using Microsoft.EntityFrameworkCore;
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

        public async Task<AccountDTO> GetAccountLogin(string email, string password)
        {
            var account = await context.Accounts.FirstOrDefaultAsync(x =>
                x.Email.Equals(email) && x.Password.Equals(password)
            );
            if (account != null)
            {
                return mapper.Map<AccountDTO>(account);
            }
            return null;
        }

        public async Task<string> GetRoleNameByEmail(string email)
        {
            var roleNames = await context
                .Accounts.Include(x => x.Role)
                .Where(x => x.Email == email)
                .Select(ur => ur.Role.RoleName)
                .FirstOrDefaultAsync();

            return roleNames;
        }

        public async Task<UserDTO> GetUserByEmail(string email)
        {
            var user = await context
                .Accounts.Include(x => x.User)
                .FirstOrDefaultAsync(x => x.Email == email);
            return user != null ? mapper.Map<UserDTO>(user.User) : null;
        }
    }
}
