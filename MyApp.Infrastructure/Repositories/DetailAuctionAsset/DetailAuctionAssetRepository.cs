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
            int totalDocuments = await _context
                .AuctionDocuments.Where(a => a.StatusDeposit == 1)
                .CountAsync(a => a.AuctionAsset.AuctionAssetsId == auctionAssetsId);
            decimal depositByAuctionAssetsId = await _context
                .AuctionAssets.Where(a => a.AuctionAssetsId == auctionAssetsId)
                .Select(x => x.Deposit)
                .FirstOrDefaultAsync();
            return depositByAuctionAssetsId * totalDocuments;
        }

        public async Task<decimal> GetTotalRegistrationFeeAsync(Guid auctionAssetsId)
        {
            int totalDocuments = await _context
                .AuctionDocuments.Where(a => a.StatusTicket == 2)
                .CountAsync(a => a.AuctionAsset.AuctionAssetsId == auctionAssetsId);
            decimal registrationFeeByAuctionAssetsId = await _context
                .AuctionAssets.Where(a => a.AuctionAssetsId == auctionAssetsId)
                .Select(x => x.RegistrationFee)
                .FirstOrDefaultAsync();
            return registrationFeeByAuctionAssetsId * totalDocuments;
        }
    }
}
