using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyApp.Application.Interfaces.IActionAssetsRepository;
using MyApp.Infrastructure.Data;

namespace MyApp.Infrastructure.Repositories.AuctionAssetsImplement
{
    public class AuctionAssetsImplement : IAuctionAssetsRepository
    {
        private readonly AppDbContext _context;

        public AuctionAssetsImplement(AppDbContext context)
        {
            _context = context;
        }

        public Task DeleteByAuctionIdAsync(Guid auctionId)
        {
            var assets = _context.AuctionAssets.Where(a => a.AuctionId == auctionId);
            _context.AuctionAssets.RemoveRange(assets);

            return Task.CompletedTask;
        }
    }
}
