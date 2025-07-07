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

        public async Task<(bool, string, string)> AssignAuctioneerToAuctionAndPublicAuctionAsync(
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
                return (
                    true,
                    auction.AuctionEndDate.ToString(),
                    auction.AuctionStartDate.ToString()
                ); // successfully assigned auctioneer to auction
            }
            catch (Exception)
            {
                return (false, null, null); // failed to assign auctioneer to auction
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

        public async Task GenerateNumbericalOrderAsync(Guid auctionId)
        {
            // Lấy tất cả hồ sơ trong phiên đấu
            var documents = await _context
                .AuctionDocuments.Include(ad => ad.AuctionAsset)
                .Where(ad => ad.AuctionAsset.AuctionId == auctionId)
                .Where(ad => ad.StatusTicket == 2) // Chỉ lấy những hồ sơ đã ký phiếu đăng ký hồ sơ
                .Where(ad => ad.StatusDeposit == 1) // Chỉ lấy những hồ sơ đã cọc
                .ToListAsync();

            // Nhóm theo UserId để đảm bảo 1 user chỉ được đánh 1 STT trong 1 phiên
            var groupedByUser = documents.GroupBy(doc => doc.UserId).ToList();

            int stt = 1;

            foreach (var group in groupedByUser)
            {
                var userDocs = group.ToList();

                // Nếu tất cả các hồ sơ của user này chưa có STT
                if (userDocs.All(d => d.NumericalOrder == null || d.NumericalOrder == 0))
                {
                    foreach (var doc in userDocs)
                    {
                        doc.NumericalOrder = stt;
                    }
                    stt++;
                }
            }

            await _context.SaveChangesAsync();
        }
    }
}
