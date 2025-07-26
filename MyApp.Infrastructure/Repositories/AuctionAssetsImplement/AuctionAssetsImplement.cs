using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using MyApp.Application.Interfaces.IActionAssetsRepository;
using MyApp.Core.DTOs.AuctionAssetsDTO;
using MyApp.Infrastructure.Data;

namespace MyApp.Infrastructure.Repositories.AuctionAssetsImplement
{
    public class AuctionAssetsImplement : IAuctionAssetsRepository
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuctionAssetsImplement(
            AppDbContext context,
            IHttpContextAccessor httpContextAccessor
        )
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public Task DeleteByAuctionIdAsync(Guid auctionId)
        {
            var assets = _context.AuctionAssets.Where(a => a.AuctionId == auctionId);
            _context.AuctionAssets.RemoveRange(assets);

            return Task.CompletedTask;
        }

        public async Task<
            PagedResult<AuctionAssetsWithHighestBidResponse>
        > GetAuctionAssetsWithHighestBidByAuctionIdAsync(
            Guid auctionId,
            string? tagName,
            int pageNumber,
            int pageSize
        )
        {
            var auctionExists = await _context.Auctions.AnyAsync(a => a.AuctionId == auctionId);
            if (!auctionExists)
            {
                throw new KeyNotFoundException("Phiên đấu giá không tồn tại.");
            }

            var user = _httpContextAccessor.HttpContext?.User;
            var userIdStr = user
                ?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)
                ?.Value;
            Guid.TryParse(userIdStr, out Guid userId);

            var account = await _context
                .Accounts.Where(a => a.UserId == userId)
                .Select(a => new { a.RoleId })
                .FirstOrDefaultAsync();

            var userRoleId = account?.RoleId ?? 0;

            var query = _context.AuctionAssets.Where(a => a.AuctionId == auctionId);
            if (!string.IsNullOrEmpty(tagName))
            {
                query = query.Where(a => a.TagName.Contains(tagName));
            }

            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var auctionAssets = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var winningBids = await _context
                .AuctionRoundPrices.Where(p =>
                    p.FlagWinner == true
                    && _context.AuctionRounds.Any(r =>
                        r.AuctionRoundId == p.AuctionRoundId && r.AuctionId == auctionId
                    )
                )
                .ToListAsync();

            var winnerBidMap = winningBids
                .GroupBy(p => p.TagName)
                .Select(g => g.First())
                .ToDictionary(
                    b => b.TagName,
                    b =>
                    {
                        var info = new HighestBidInfo { Price = b.AuctionPrice };
                        if (userRoleId == 3 || userRoleId == 6)
                        {
                            info.Name = b.UserName;
                            info.CitizenIdentification = b.CitizenIdentification;
                        }
                        return info;
                    }
                );

            var items = auctionAssets
                .Select(asset => new AuctionAssetsWithHighestBidResponse
                {
                    AuctionAssetsId = asset.AuctionAssetsId,
                    TagName = asset.TagName,
                    StartingPrice = asset.StartingPrice,
                    Unit = asset.Unit,
                    Deposit = asset.Deposit,
                    RegistrationFee = asset.RegistrationFee,
                    Description = asset.Description,
                    CreatedAt = asset.CreatedAt,
                    CreatedBy = asset.CreatedBy,
                    UpdatedAt = asset.UpdatedAt,
                    UpdatedBy = asset.UpdatedBy,
                    AuctionId = asset.AuctionId,
                    HighestBid = winnerBidMap.TryGetValue(asset.TagName, out var bid) ? bid : null,
                })
                .ToList();

            return new PagedResult<AuctionAssetsWithHighestBidResponse>
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalItems = totalItems,
                TotalPages = totalPages,
                Items = items,
            };
        }
    }
}
