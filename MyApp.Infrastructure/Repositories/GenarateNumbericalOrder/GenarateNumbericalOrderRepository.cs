using Microsoft.EntityFrameworkCore;
using MyApp.Application.Interfaces.GenarateNumbericalOrder;
using MyApp.Infrastructure.Data;

namespace MyApp.Infrastructure.Repositories.GenarateNumbericalOrder
{
    public class GenarateNumbericalOrderRepository : IGenarateNumbericalOrderRepository
    {
        private readonly AppDbContext _context;

        public GenarateNumbericalOrderRepository(AppDbContext context)
        {
            this._context = context;
        }

        public async Task<bool> GenerateNumbericalOrderAsync(Guid auctionId)
        {
            // Lấy tất cả hồ sơ trong phiên đấu
            var documents = await _context
                .AuctionDocuments.Include(ad => ad.AuctionAsset)
                .Where(ad => ad.AuctionAsset.AuctionId == auctionId)
                .Where(ad => ad.StatusTicket == 2) // Chỉ lấy những hồ sơ đã ký phiếu đăng ký hồ sơ
                .Where(ad => ad.StatusDeposit == 1) // Chỉ lấy những hồ sơ đã cọc
                .ToListAsync();

            // Lấy số thứ tự lớn nhất đã được gán (nếu có), mặc định là 0
            int maxNumericalOrder = documents
                .Where(d => d.NumericalOrder != null && d.NumericalOrder != 0)
                .Select(d => d.NumericalOrder.Value)
                .DefaultIfEmpty(0)
                .Max();

            int stt = maxNumericalOrder + 1;

            // Nhóm theo UserId để đảm bảo 1 user chỉ được đánh 1 STT trong 1 phiên
            var groupedByUser = documents.GroupBy(doc => doc.UserId).ToList();

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
                else
                {
                    // Nếu đã có STT, thì gán lại STT đã có cho những hồ sơ chưa có
                    var existingOrder = userDocs
                        .FirstOrDefault(d => d.NumericalOrder != null && d.NumericalOrder != 0)
                        ?.NumericalOrder;

                    if (existingOrder != null && existingOrder != 0)
                    {
                        foreach (
                            var doc in userDocs.Where(d =>
                                d.NumericalOrder == null || d.NumericalOrder == 0
                            )
                        )
                        {
                            doc.NumericalOrder = existingOrder;
                        }
                    }
                }
            }

            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
