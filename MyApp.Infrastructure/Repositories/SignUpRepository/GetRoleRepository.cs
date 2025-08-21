using Microsoft.EntityFrameworkCore;
using MyApp.Application.Interfaces.ISignUpRepository;
using MyApp.Core.Entities;
using MyApp.Infrastructure.Data;

namespace MyApp.Infrastructure.Repositories.SignUpRepository
{
    public class GetRoleRepository : IGetRoleRepository
    {
        private readonly AppDbContext context;

        public GetRoleRepository(AppDbContext context)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<List<Role>?> GetAllRolesAsync()
        {
            var roles = await context.Roles.Where(r => r.RoleId != 2).ToListAsync();
            return roles.Any() ? roles : null;
        }
    }
}
