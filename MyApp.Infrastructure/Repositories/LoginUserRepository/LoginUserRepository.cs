using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MyApp.Application.Interfaces.ILoginUserRepository;
using MyApp.Core.DTOs.LoginUserDTO;
using MyApp.Infrastructure.Data;

namespace MyApp.Infrastructure.Repositories.LoginUserRepository
{
    public class LoginUserRepository : ILoginUserRepository
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public LoginUserRepository(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<AccountDTO> GetAccountLogin(string email, string password)
        {
            var account = await _context.Accounts.FirstOrDefaultAsync(x =>
                x.Email == email && x.Password == password
            );
            return account != null ? _mapper.Map<AccountDTO>(account) : null;
        }

        public async Task<string> GetRoleNameByEmail(string email)
        {
            return await _context
                .Accounts.Include(a => a.Role)
                .Where(a => a.Email == email)
                .Select(a => a.Role.RoleName)
                .FirstOrDefaultAsync();
        }

        public async Task<UserDTO> GetUserByEmail(string email)
        {
            var account = await _context
                .Accounts.Include(a => a.User)
                .FirstOrDefaultAsync(a => a.Email == email);
            return account?.User != null ? _mapper.Map<UserDTO>(account.User) : null;
        }
    }
}
