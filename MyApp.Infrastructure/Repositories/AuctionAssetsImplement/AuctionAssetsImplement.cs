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
            List<AuctionAssetsWithHighestBidResponse>
        > GetAuctionAssetsWithHighestBidByAuctionIdAsync(Guid auctionId)
        {
            var auctionExists = await _context.Auctions.AnyAsync(a => a.AuctionId == auctionId);
            if (!auctionExists)
            {
                throw new KeyNotFoundException("Phiên đấu giá không tồn tại.");
            }

            // Lấy userId từ HttpContext (Claim)
            var user = _httpContextAccessor.HttpContext?.User;
            var userIdStr = user
                ?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)
                ?.Value;

            Guid.TryParse(userIdStr, out Guid userId);

            // Truy vấn DB để lấy role từ bảng Account
            var account = await _context
                .Accounts.Where(a => a.UserId == userId)
                .Select(a => new { a.RoleId })
                .FirstOrDefaultAsync();

            var userRoleId = account?.RoleId ?? 0;

            // Bước 1: Lấy toàn bộ tài sản thuộc phiên đấu giá
            var auctionAssets = await _context
                .AuctionAssets.Where(a => a.AuctionId == auctionId)
                .ToListAsync();

            // Bước 2: Lấy các bản ghi có FlagWinner = true trong toàn bộ các vòng của phiên
            var winningBids = await _context
                .AuctionRoundPrices.Where(p =>
                    p.FlagWinner == true
                    && _context.AuctionRounds.Any(r =>
                        r.AuctionRoundId == p.AuctionRoundId && r.AuctionId == auctionId
                    )
                )
                .ToListAsync();

            // Tạo từ điển ánh xạ TagName -> HighestBidInfo
            var winnerBidMap = winningBids
                .GroupBy(p => p.TagName)
                .Select(g => g.First()) // mỗi tài sản có 1 người thắng
                .ToDictionary(
                    b => b.TagName,
                    b =>
                    {
                        var info = new HighestBidInfo { Price = b.AuctionPrice };

                        // Nếu role là 3 hoặc 6 thì gán thêm Name và CitizenIdentification
                        if (userRoleId == 3 || userRoleId == 6)
                        {
                            info.Name = b.UserName;
                            info.CitizenIdentification = b.CitizenIdentification;
                        }

                        return info;
                    }
                );

            // Bước 3: Gộp dữ liệu tài sản và thông tin người thắng
            var result = auctionAssets
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

                    HighestBid = winnerBidMap.ContainsKey(asset.TagName)
                        ? winnerBidMap[asset.TagName]
                        : null,
                })
                .ToList();

            return result;
        }
    }
}
