using Microsoft.EntityFrameworkCore;
using MyApp.Application.Common.Message;
using MyApp.Application.CQRS.SearchUserAttendance.Queries;
using MyApp.Application.Interfaces.SearchUserAttendance;
using MyApp.Infrastructure.Data;

namespace MyApp.Infrastructure.Repositories.SearchUserAttendance
{
    public class SearchUserAttendanceRepository : ISearchUserAttendanceRepository
    {
        private readonly AppDbContext context;

        public SearchUserAttendanceRepository(AppDbContext context)
        {
            this.context = context;
        }

        public async Task<int?> GetNumericalOrderAsync(Guid auctionId, string citizenIdentification)
        {
            return await context
                .AuctionDocuments.Include(x => x.AuctionAsset)
                .ThenInclude(x => x.Auction)
                .Where(x => x.AuctionAsset.Auction.AuctionId == auctionId)
                .Include(x => x.User)
                .Where(x => x.User.CitizenIdentification == citizenIdentification)
                .Where(x => x.StatusTicket == 2 && x.StatusDeposit == 1)
                .Select(x => x.NumericalOrder)
                .FirstOrDefaultAsync();
        }

        public async Task<string?> GetAuctionNameAsync(Guid auctionId)
        {
            return await context
                .Auctions.Where(a => a.AuctionId == auctionId)
                .Select(a => a.AuctionName)
                .FirstOrDefaultAsync();
        }
    }
}
