using Microsoft.EntityFrameworkCore;
using MyApp.Application.CQRS.ListAuctionRegisted;
using MyApp.Application.Interfaces.ListAuctionRegisted;
using MyApp.Infrastructure.Data;

namespace MyApp.Infrastructure.Repositories.ListAuctionRegisted
{
    public class AuctionRegistedRepository : IAuctionRegistedRepository
    {
        private readonly AppDbContext _context;

        public AuctionRegistedRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Guid>> GetRegisteredAssetIdsAsync(Guid userId)
        {
            return await _context
                .AuctionDocuments.Where(ad => ad.UserId == userId && ad.StatusTicket != 0)
                .Select(ad => ad.AuctionAssetId)
                .ToListAsync();
        }

        public async Task<List<Guid>> GetRegisteredAuctionIdsAsync(List<Guid> assetIds)
        {
            return await _context
                .AuctionAssets.Where(aa => assetIds.Contains(aa.AuctionAssetsId))
                .Select(aa => aa.AuctionId)
                .Distinct()
                .ToListAsync();
        }

        public async Task<List<AuctionResponse>> GetAuctionsByIdsAsync(List<Guid> auctionIds)
        {
            return await _context
                .Auctions.Where(a => auctionIds.Contains(a.AuctionId))
                .Include(a => a.Category)
                .Select(a => new AuctionResponse
                {
                    AuctionId = a.AuctionId,
                    AuctionName = a.AuctionName,
                    CategoryName = a.Category.CategoryName,
                    AuctionDescription = a.AuctionDescription,
                    AuctionRules = a.AuctionRules,
                    AuctionPlanningMap = a.AuctionPlanningMap,
                    RegisterOpenDate = a.RegisterOpenDate,
                    RegisterEndDate = a.RegisterEndDate,
                    AuctionStartDate = a.AuctionStartDate,
                    AuctionEndDate = a.AuctionEndDate,
                    NumberRoundMax = a.NumberRoundMax,
                    Status = a.Status,
                })
                .ToListAsync();
        }

        public async Task<List<AuctionAsset>> GetAuctionAssetsByAuctionIdAsync(
            Guid auctionId,
            List<Guid> assetIds
        )
        {
            return await _context
                .AuctionAssets.Where(aa =>
                    aa.AuctionId == auctionId && assetIds.Contains(aa.AuctionAssetsId)
                )
                .Select(aa => new AuctionAsset
                {
                    AuctionAssetsId = aa.AuctionAssetsId,
                    TagName = aa.TagName,
                    StartingPrice = aa.StartingPrice,
                    Unit = aa.Unit,
                    Deposit = aa.Deposit,
                    RegistrationFee = aa.RegistrationFee,
                    Description = aa.Description,
                    AuctionId = aa.AuctionId,
                })
                .ToListAsync();
        }
    }
}
