using Microsoft.EntityFrameworkCore;
using MyApp.Application.Interfaces.AssginAuctioneerAndPublicAuction;
using MyApp.Infrastructure.Data;

namespace MyApp.Infrastructure.Repositories.AssginAuctioneerAndPublicAuction
{
    public class AssginAuctioneerAndPublicAuctionRepository
        : IAssginAuctioneerAndPublicAuctionRepository
    {
        private readonly AppDbContext _context;

        public AssginAuctioneerAndPublicAuctionRepository(AppDbContext context)
        {
            this._context = context;
        }

        public async Task<(bool, string)> AssignAuctioneerToAuctionAndPublicAuctionAsync(
            Guid auctionId,
            Guid auctioneerId,
            string userId
        )
        {
            var auction = await _context.Auctions.FirstOrDefaultAsync(x =>
                x.AuctionId == auctionId
            );

            try
            {
                auction.Auctioneer = auctioneerId;
                auction.Status = 1; // public auction status
                auction.UpdatedAt = DateTime.UtcNow;
                auction.UpdatedBy = Guid.Parse(userId);
                _context.Auctions.Update(auction);
                await _context.SaveChangesAsync();
                return (true, auction.AuctionEndDate.ToString()); // successfully assigned auctioneer to auction
            }
            catch (Exception)
            {
                return (false, null); // failed to assign auctioneer to auction
            }
        }

        public async Task<bool> CheckAuctioneerAssignedToAnotherAuctionAsync(
            Guid auctioneerId,
            Guid auctionId
        )
        {
            //get auction by auctioneerId and check if it is not null
            var auction = await _context.Auctions.FirstOrDefaultAsync(x =>
                x.AuctionId == auctionId
            );
            var check = await _context.Auctions.AnyAsync(x =>
                x.Auctioneer == auctioneerId
                && x.AuctionId != auctionId
                && x.AuctionStartDate == auction.AuctionStartDate
                && x.AuctionEndDate == auction.AuctionEndDate
            );
            if (check)
            {
                return true; // auctioneer is assigned to another auction
            }
            return false; // auctioneer is not assigned to another auction
        }
    }
}
