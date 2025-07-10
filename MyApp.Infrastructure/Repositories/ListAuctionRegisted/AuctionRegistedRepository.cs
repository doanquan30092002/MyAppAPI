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

        public async Task<List<AuctionRegistedResponse>?> ListAuctionRegisted(string? userId)
        {
            if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var userGuid))
                return null;

            // Get all AuctionAssetIds the user has registered in AuctionDocuments
            var registeredAssetIds = await _context
                .AuctionDocuments.Where(ad =>
                    ad.UserId == userGuid && ad.StatusTicket == 2 && ad.StatusDeposit == 1
                )
                .Select(ad => ad.AuctionAssetId)
                .ToListAsync();

            if (!registeredAssetIds.Any())
                return null;

            // Get all AuctionIds from AuctionAssets the user has registered
            var registeredAuctionIds = await _context
                .AuctionAssets.Where(aa => registeredAssetIds.Contains(aa.AuctionAssetsId))
                .Select(aa => aa.AuctionId)
                .Distinct()
                .ToListAsync();

            if (!registeredAuctionIds.Any())
                return null;

            // Get info of all auctions the user has registered
            var auctions = await _context
                .Auctions.Where(a => registeredAuctionIds.Contains(a.AuctionId))
                .Select(a => new AuctionRegistedResponse
                {
                    AuctionId = a.AuctionId,
                    AuctionName = a.AuctionName,
                    CategoryName = a.Category.CategoryName,
                    AuctionDescription = a.AuctionDescription,
                    AuctionPlanningMap = a.AuctionPlanningMap,
                    RegisterOpenDate = a.RegisterOpenDate,
                    RegisterEndDate = a.RegisterEndDate,
                    AuctionStartDate = a.AuctionStartDate,
                    AuctionEndDate = a.AuctionEndDate,
                    NumberRoundMax = a.NumberRoundMax,
                    Status = a.Status,
                    AuctionAssets = _context
                        .AuctionAssets.Where(aa => aa.AuctionId == a.AuctionId)
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
                        .ToList(),
                })
                .ToListAsync();

            return auctions;
        }
    }
}
