using Microsoft.EntityFrameworkCore;
using MyApp.Application.CQRS.GetAuctioneers.Queries;
using MyApp.Application.Interfaces.GetAuctioneers;
using MyApp.Infrastructure.Data;

namespace MyApp.Infrastructure.Repositories.GetAuctioneers
{
    public class GetAuctioneersRepository : IGetAuctioneersRepository
    {
        private readonly AppDbContext _context;

        public GetAuctioneersRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<GetAuctioneersResponse>> GetAuctioneersAsync()
        {
            var auctioneers = await _context
                .Accounts.Include(x => x.User)
                .Where(x => x.RoleId == 4)
                .Select(x => new GetAuctioneersResponse { Name = x.User.Name, Id = x.User.Id })
                .ToListAsync();
            return auctioneers;
        }
    }
}
