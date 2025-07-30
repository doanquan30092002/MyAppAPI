using Microsoft.EntityFrameworkCore;
using MyApp.Application.Interfaces.ChangeStatusAuctionRound;
using MyApp.Infrastructure.Data;

namespace MyApp.Infrastructure.Repositories.ChangeStatusAuctionRound
{
    public class ChangeStatusAuctionRoundRepository : IChangeStatusAuctionRoundRepository
    {
        private readonly AppDbContext _context;

        public ChangeStatusAuctionRoundRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> ChangeStatusAuctionRoundAsync(Guid auctionRoundId, int status)
        {
            var auctionRound = await _context.AuctionRounds.FirstOrDefaultAsync(ar =>
                ar.AuctionRoundId == auctionRoundId
            );
            if (auctionRound == null)
            {
                return false; // Không tìm thấy phiên đấu giá
            }
            auctionRound.Status = status;
            auctionRound.CreatedAt = DateTime.Now;
            _context.AuctionRounds.Update(auctionRound);
            try
            {
                await _context.SaveChangesAsync();
                return true; // Cập nhật thành công
            }
            catch (Exception)
            {
                return false; // Xử lý lỗi nếu có
            }
        }
    }
}
