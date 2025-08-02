using Microsoft.EntityFrameworkCore;
using MyApp.Application.Common.Message;
using MyApp.Application.CQRS.UserRegisteredAuction;
using MyApp.Application.Interfaces.UserRegisteredAuction;
using MyApp.Core.Entities;
using MyApp.Infrastructure.Data;

namespace MyApp.Infrastructure.Repositories.UserRegisteredAuction
{
    public class UserRegisteredAuctionRepository : IUserRegisteredAuctionRepository
    {
        private readonly AppDbContext _context;

        public UserRegisteredAuctionRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<
            List<(string TagName, decimal AuctionPrice)>
        > GetNextRoundTagNamesForUserAsync(Guid? auctionRoundId, string citizenIdentification)
        {
            var result = await _context
                .AuctionRoundPrices.Where(p =>
                    p.AuctionRoundId == auctionRoundId
                    && p.CitizenIdentification == citizenIdentification
                    && p.FlagWinner == true
                    && _context
                        .AuctionRoundPrices.Where(sub =>
                            sub.AuctionRoundId == p.AuctionRoundId
                            && sub.TagName == p.TagName
                            && sub.FlagWinner == true
                        )
                        .GroupBy(sub => new { sub.AuctionRoundId, sub.TagName })
                        .Any(g => g.Count() > 1)
                )
                .Select(p => new { p.TagName, p.AuctionPrice })
                .Distinct()
                .ToListAsync();
            return result.Select(r => (r.TagName, r.AuctionPrice)).ToList();
        }

        public async Task<User?> GetUserByCitizenIdAsync(string citizenId)
        {
            return await _context.Users.FirstOrDefaultAsync(u =>
                u.CitizenIdentification == citizenId
            );
        }

        public async Task<List<AuctionAsset>> GetValidAuctionAssetsAsync(
            Guid userId,
            Guid auctionId
        )
        {
            return await _context
                .AuctionDocuments.Where(ad =>
                    ad.UserId == userId
                    && ad.AuctionAsset.AuctionId == auctionId
                    && ad.StatusTicket == 2
                    && ad.StatusDeposit == 1
                )
                .Select(ad => new AuctionAsset
                {
                    AuctionAssetsId = ad.AuctionAsset.AuctionAssetsId,
                    TagName = ad.AuctionAsset.TagName,
                    StartingPrice = ad.AuctionAsset.StartingPrice,
                })
                .ToListAsync();
        }
    }
}
