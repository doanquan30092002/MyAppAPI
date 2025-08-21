using Microsoft.EntityFrameworkCore;
using MyApp.Application.CQRS.ListAuctionAsset;
using MyApp.Application.Interfaces.ListAuctionAsset;
using MyApp.Infrastructure.Data;

namespace MyApp.Infrastructure.Repositories.ListAuctionAsset
{
    public class ListAuctionAssetRepository : IListAuctionAssetRepository
    {
        private readonly AppDbContext _context;

        public ListAuctionAssetRepository(AppDbContext context)
        {
            _context = context;
        }

        public Task<List<AuctionAssetResponse>> GetAllAuctionAssets(
            int pageNumber,
            int pageSize,
            SearchAuctionAsset search
        )
        {
            var query = _context
                .AuctionAssets.Include(x => x.Auction)
                .ThenInclude(x => x.Category)
                .Where(x => x.Auction.Status == 1 || x.Auction.Status == 2 || x.Auction.Status == 3)
                .AsQueryable();
            if (search.CategoryId.HasValue)
            {
                query = query.Where(a => a.Auction.Category.CategoryId == search.CategoryId);
            }
            if (!string.IsNullOrEmpty(search.TagName))
            {
                query = query.Where(a => a.TagName.Contains(search.TagName));
            }
            if (search.AuctionStartDate.HasValue)
            {
                query = query.Where(a => a.Auction.CreatedAt >= search.AuctionStartDate);
            }
            if (search.AuctionEndDate.HasValue)
            {
                query = query.Where(a => a.Auction.CreatedAt <= search.AuctionEndDate);
            }
            if (search.AuctionStatus.HasValue)
            {
                query = query.Where(a => a.Auction.Status == search.AuctionStatus);
            }
            return query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(a => new AuctionAssetResponse
                {
                    AuctionAssetsId = a.AuctionAssetsId,
                    TagName = a.TagName,
                    StartingPrice = a.StartingPrice,
                    Unit = a.Unit,
                    Deposit = a.Deposit,
                    RegistrationFee = a.RegistrationFee,
                    Description = a.Description,
                    CreatedAt = a.Auction.CreatedAt,
                    CreatedBy = a.Auction.CreatedBy,
                    UpdatedAt = a.Auction.UpdatedAt,
                    UpdatedBy = a.Auction.UpdatedBy,
                    AuctionId = a.AuctionId,
                    AuctionName = a.Auction.AuctionName,
                    CategoryName = a.Auction.Category.CategoryName,
                })
                .ToListAsync();
        }

        public Task<Dictionary<string, int>> GetCategoryCounts()
        {
            var categoryCounts = _context
                .AuctionAssets.Include(x => x.Auction)
                .ThenInclude(x => x.Category)
                .Where(x => x.Auction.Status == 1 || x.Auction.Status == 2 || x.Auction.Status == 3)
                .GroupBy(x => x.Auction.Category.CategoryName)
                .Select(g => new { CategoryName = g.Key, Count = g.Count() })
                .ToDictionaryAsync(g => g.CategoryName, g => g.Count);
            return categoryCounts;
        }

        public Task<int> GetTotalAuctionAssetsCount()
        {
            var totalCount = _context
                .AuctionAssets.Include(x => x.Auction)
                .ThenInclude(x => x.Category)
                .Where(x => x.Auction.Status == 1 || x.Auction.Status == 2 || x.Auction.Status == 3)
                .CountAsync();
            return totalCount;
        }
    }
}
