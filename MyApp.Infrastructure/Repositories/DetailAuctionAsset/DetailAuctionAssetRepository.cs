using Microsoft.EntityFrameworkCore;
using MyApp.Application.CQRS.DetailAuctionAsset;
using MyApp.Application.Interfaces.DetailAuctionAsset;
using MyApp.Infrastructure.Data;

namespace MyApp.Infrastructure.Repositories.DetailAuctionAsset
{
    public class DetailAuctionAssetRepository : IDetailAuctionAssetRepository
    {
        private readonly AppDbContext _context;

        public DetailAuctionAssetRepository(AppDbContext context)
        {
            _context = context;
        }

        public Task<AuctionAssetResponse?> GetAuctionAssetByIdAsync(Guid auctionAssetsId)
        {
            var auctionAsset = _context
                .AuctionAssets.Include(x => x.Auction)
                .Where(a => a.AuctionAssetsId == auctionAssetsId)
                .Select(a => new AuctionAssetResponse
                {
                    AuctionAssetsId = a.AuctionAssetsId,
                    TagName = a.TagName,
                    StartingPrice = a.StartingPrice,
                    Unit = a.Unit,
                    Deposit = a.Deposit,
                    RegistrationFee = a.RegistrationFee,
                    Description = a.Description,
                    CreatedAt = a.CreatedAt,
                    AuctionName = a.Auction.AuctionName,
                })
                .FirstOrDefaultAsync();
            return auctionAsset;
        }

        public async Task<int> GetTotalAuctionDocumentsAsync(Guid auctionAssetsId)
        {
            return await _context.AuctionDocuments.CountAsync(ad =>
                ad.AuctionAssetId == auctionAssetsId
            );
        }

        public async Task<decimal> GetTotalDepositAsync(Guid auctionAssetsId)
        {
            return await _context
                .AuctionAssets.Where(a => a.AuctionAssetsId == auctionAssetsId)
                .SumAsync(a => a.Deposit);
        }

        public Task<decimal> GetTotalRegistrationFeeAsync(Guid auctionAssetsId)
        {
            return _context
                .AuctionAssets.Where(a => a.AuctionAssetsId == auctionAssetsId)
                .SumAsync(a => a.RegistrationFee);
        }
    }
}
