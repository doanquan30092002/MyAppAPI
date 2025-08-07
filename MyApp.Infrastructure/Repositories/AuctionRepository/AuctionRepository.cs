using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using MyApp.Application.Common.Services.UploadFile;
using MyApp.Application.CQRS.Auction.AddAuction.Commands;
using MyApp.Application.CQRS.Auction.CancelAuction.Commands;
using MyApp.Application.CQRS.Auction.UpdateAuction.Commands;
using MyApp.Application.Interfaces.IActionAssetsRepository;
using MyApp.Application.Interfaces.IAuctionRepository;
using MyApp.Core.Entities;
using MyApp.Infrastructure.Data;

namespace MyApp.Infrastructure.Repositories.AuctionRepository
{
    public class AuctionRepository : IAuctionRepository
    {
        private readonly AppDbContext _context;
        private readonly IUploadFile _uploadFileService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAuctionAssetsRepository _auctionAssetsRepository;

        public AuctionRepository(
            AppDbContext context,
            IUploadFile uploadFileService,
            IHttpContextAccessor httpContextAccessor,
            IAuctionAssetsRepository auctionAssetsRepository
        )
        {
            _context = context;
            _uploadFileService = uploadFileService;
            _httpContextAccessor = httpContextAccessor;
            _auctionAssetsRepository = auctionAssetsRepository;
        }

        public async Task<Guid> AddAuctionAsync(AddAuctionCommand command, Guid userId)
        {
            string? auctionRulesUrl = null;
            string? auctionPlanningMapUrl = null;

            if (command.AuctionRulesFile != null && command.AuctionRulesFile.Length > 0)
            {
                auctionRulesUrl = await _uploadFileService.UploadAsync(command.AuctionRulesFile);
            }

            if (command.AuctionPlanningMap != null && command.AuctionPlanningMap.Length > 0)
            {
                auctionPlanningMapUrl = await _uploadFileService.UploadAsync(
                    command.AuctionPlanningMap
                );
            }

            var auction = new Auction
            {
                AuctionId = Guid.NewGuid(),
                AuctionName = command.AuctionName,
                AuctionDescription = command.AuctionDescription,
                AuctionRules = auctionRulesUrl ?? "No file uploaded",
                AuctionPlanningMap = auctionPlanningMapUrl ?? "No file uploaded",
                RegisterOpenDate = command.RegisterOpenDate,
                RegisterEndDate = command.RegisterEndDate,
                AuctionStartDate = command.AuctionStartDate,
                AuctionEndDate = command.AuctionEndDate,
                AuctionMap = command.Auction_Map,
                CreatedAt = DateTime.Now,
                CreatedBy = userId,
                UpdatedAt = DateTime.Now,
                UpdatedBy = userId,
                QRLink = "Test",
                NumberRoundMax = command.NumberRoundMax,
                Status = 0,
                WinnerData = null,
                CategoryId = command.CategoryId,
                Updateable = true,
                CancelReasonFile = "No file uploaded",
                CancelReason = null,
            };

            await _context.Auctions.AddAsync(auction);

            return auction.AuctionId;
        }

        public async Task<bool> CancelAuctionAsync(CancelAuctionCommand command, Guid userId)
        {
            var auction = await FindAuctionByIdAsync(command.AuctionId);
            if (auction == null)
            {
                throw new ValidationException("Phiên đấu giá không tồn tại.");
            }

            if (auction.Status != 1)
            {
                throw new ValidationException(
                    "Chỉ được phép huỷ phiên đấu giá khi trạng thái là công khai."
                );
            }

            string? cancelReasonFileUrl = auction.CancelReasonFile;
            if (command.CancelReasonFile != null && command.CancelReasonFile.Length > 0)
            {
                cancelReasonFileUrl = await _uploadFileService.UploadAsync(
                    command.CancelReasonFile
                );
            }

            auction.CancelReason = command.CancelReason;
            auction.CancelReasonFile = cancelReasonFileUrl ?? "No file uploaded";
            auction.Status = 3;
            auction.Updateable = false;
            auction.UpdatedAt = DateTime.Now;
            auction.UpdatedBy = userId;

            _context.Auctions.Update(auction);

            return true;
        }

        public async Task<Auction?> FindAuctionByIdAsync(Guid auctionId)
        {
            return await _context.Auctions.FirstOrDefaultAsync(a => a.AuctionId == auctionId);
        }

        public async Task<bool> UpdateAuctionAsync(UpdateAuctionCommand command, Guid userId)
        {
            var auction = await _context.Auctions.FirstOrDefaultAsync(a =>
                a.AuctionId == command.AuctionId
            );
            if (auction == null)
                throw new ValidationException("Phiên đấu giá không tồn tại.");

            int oldStatus = auction.Status;
            int newStatus = command.Status;

            if (newStatus == 1)
            {
                var account = await _context.Accounts.FirstOrDefaultAsync(a => a.UserId == userId);

                if (account == null)
                    throw new ValidationException("Tài khoản không tồn tại.");

                if (account.RoleId != 4)
                    throw new ValidationException("Bạn không có quyền công khai phiên đấu giá.");

                if (DateTime.Now > auction.AuctionEndDate)
                    throw new ValidationException(
                        "Không thể công khai phiên đấu giá do ngày kết thúc phiên đấu giá đã qua."
                    );
            }

            if (oldStatus == 1)
            {
                if (command.WinnerData != null)
                {
                    auction.WinnerData = command.WinnerData;
                    auction.UpdatedAt = DateTime.Now;
                    auction.UpdatedBy = userId;
                }
            }
            else
            {
                if (command.AuctionRulesFile != null && command.AuctionRulesFile.Length > 0)
                {
                    auction.AuctionRules = await _uploadFileService.UploadAsync(
                        command.AuctionRulesFile
                    );
                }

                if (command.AuctionPlanningMap != null && command.AuctionPlanningMap.Length > 0)
                {
                    auction.AuctionPlanningMap = await _uploadFileService.UploadAsync(
                        command.AuctionPlanningMap
                    );
                }
                else
                {
                    auction.AuctionPlanningMap = "No file uploaded";
                }

                auction.AuctionMap = command.Auction_Map;
                auction.AuctionName = command.AuctionName;
                auction.AuctionDescription = command.AuctionDescription;
                auction.RegisterOpenDate = command.RegisterOpenDate;
                auction.RegisterEndDate = command.RegisterEndDate;
                auction.AuctionStartDate = command.AuctionStartDate;
                auction.AuctionEndDate = command.AuctionEndDate;
                auction.UpdatedAt = DateTime.Now;
                auction.UpdatedBy = userId;
                auction.NumberRoundMax = command.NumberRoundMax;
                auction.Status = newStatus;
                auction.CategoryId = command.CategoryId;
            }

            _context.Auctions.Update(auction);

            return true;
        }

        public async Task UpdateAuctionUpdateableAsync(Guid auctionId, bool updateable)
        {
            var auction = await _context.Auctions.FirstOrDefaultAsync(a =>
                a.AuctionId == auctionId
            );
            if (auction == null)
                throw new ValidationException("Auction không tồn tại.");

            auction.Updateable = updateable;
            auction.UpdatedAt = DateTime.Now;
            _context.Auctions.Update(auction);
            await _context.SaveChangesAsync();
        }

        public async Task<List<AuctionDocuments>> GetPaidOrDepositedDocumentsByAuctionIdAsync(
            Guid auctionId
        )
        {
            // 1. Lấy danh sách tài sản cùng với người đấu giá cao nhất
            var auctionAssetsWithWinners =
                await _auctionAssetsRepository.GetAuctionAssetsWithHighestBidByAuctionIdAsync(
                    auctionId
                );

            // 2. Lấy danh sách số căn cước của người thắng thầu (bỏ null và trùng)
            var excludedCitizenIds = auctionAssetsWithWinners
                .Where(a =>
                    a.HighestBid != null
                    && !string.IsNullOrWhiteSpace(a.HighestBid.CitizenIdentification)
                )
                .Select(a => a.HighestBid.CitizenIdentification.Trim())
                .Distinct()
                .ToList();

            // 3. Lấy danh sách các AuctionAssetId của phiên đấu giá này
            var auctionAssetIds = await _context
                .AuctionAssets.Where(x => x.AuctionId == auctionId)
                .Select(x => x.AuctionAssetsId)
                .ToListAsync();

            // 4. Lấy danh sách hồ sơ thỏa điều kiện thanh toán/đặt cọc/tham dự nhưng không nằm trong danh sách người thắng
            var documents = await _context
                .AuctionDocuments.Where(doc =>
                    auctionAssetIds.Contains(doc.AuctionAssetId)
                    && (
                        doc.StatusTicket == 1
                        || doc.StatusDeposit == 1
                        || doc.StatusRefund == 1
                        || doc.IsAttended == true
                    )
                    && !excludedCitizenIds.Contains(doc.User.CitizenIdentification)
                )
                .Include(doc => doc.User)
                .Include(doc => doc.AuctionAsset)
                .ToListAsync();

            return documents;
        }

        public async Task<List<string>> GetEmailsByUserIdsAsync(List<Guid> userIds)
        {
            var emails = await _context
                .Accounts.Where(acc =>
                    userIds.Contains(acc.UserId) && !string.IsNullOrEmpty(acc.Email)
                )
                .Select(acc => acc.Email)
                .ToListAsync();

            return emails;
        }

        public async Task<bool> WaitingPublicAsync(Guid auctionId)
        {
            Guid? userId = null;
            var userIdStr = _httpContextAccessor
                .HttpContext?.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)
                ?.Value;

            if (Guid.TryParse(userIdStr, out var parsedGuid))
            {
                userId = parsedGuid;
            }

            if (userId == null)
                throw new UnauthorizedAccessException("Không thể lấy UserId từ người dùng.");

            var auction = await _context.Auctions.FirstOrDefaultAsync(a =>
                a.AuctionId == auctionId
            );

            if (auction == null)
            {
                throw new KeyNotFoundException("Không tìm thấy phiên đấu giá.");
            }

            // Chỉ cho phép chuyển từ Draft (0) hoặc Rejected (6) sang Waiting Public (4)
            if (auction.Status != 0 && auction.Status != 6)
            {
                throw new ValidationException(
                    "Chỉ những phiên đấu giá ở trạng thái bản nháp hoặc bị từ chối mới có thể chuyển sang chờ công bố."
                );
            }

            // Kiểm tra thời điểm hiện tại phải trước ngày bắt đầu đấu giá
            if (DateTime.Now >= auction.AuctionStartDate)
            {
                throw new ValidationException(
                    "Không thể chuyển sang trạng thái chờ công bố vì đã quá thời gian bắt đầu đấu giá."
                );
            }

            auction.Status = 4; // Trạng thái chờ công bố
            auction.UpdatedAt = DateTime.Now;
            auction.RejectReason = null;
            auction.UpdatedBy = userId.Value;
            auction.Updateable = false;

            _context.Auctions.Update(auction);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> RejectAuctionAsync(Guid auctionId, string rejectReason)
        {
            Guid? userId = null;
            var userIdStr = _httpContextAccessor
                .HttpContext?.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)
                ?.Value;

            if (Guid.TryParse(userIdStr, out var parsedGuid))
            {
                userId = parsedGuid;
            }

            if (userId == null)
                throw new UnauthorizedAccessException("Không thể lấy UserId từ người dùng.");

            var auction = await _context.Auctions.FirstOrDefaultAsync(a =>
                a.AuctionId == auctionId
            );

            if (auction == null)
            {
                throw new KeyNotFoundException("Không tìm thấy phiên đấu giá.");
            }

            // Kiểm tra trạng thái phải là chờ công bố (4)
            if (auction.Status != 4)
            {
                throw new ValidationException(
                    "Chỉ những phiên đấu giá ở trạng thái chờ công bố mới được từ chối."
                );
            }

            auction.Status = 6; // Trạng thái bị từ chối
            auction.RejectReason = rejectReason;
            auction.UpdatedAt = DateTime.Now;
            auction.UpdatedBy = userId.Value;
            auction.Updateable = true;

            _context.Auctions.Update(auction);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> UpdateStatusAsync(Guid auctionId, int status)
        {
            var auction = await _context.Auctions.FindAsync(auctionId);
            if (auction == null)
            {
                throw new KeyNotFoundException("Không tìm thấy phiên đấu giá " + auctionId);
            }

            auction.Status = status;
            auction.UpdatedAt = DateTime.UtcNow;

            _context.Auctions.Update(auction);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> MarkAuctionAsSuccessfulAsync(Guid auctionId)
        {
            var auction = await _context.Auctions.FindAsync(auctionId);
            if (auction == null)
            {
                throw new KeyNotFoundException($"Phiên đấu giá {auctionId} Không tồn tại.");
            }

            if (auction.Status != 1)
            {
                throw new InvalidOperationException(
                    $"Chỉ có thế chuyển trạng thái từ công khai sang thành công."
                );
            }

            auction.Status = 2;
            auction.UpdatedAt = DateTime.Now;
            auction.Updateable = false;

            _context.Auctions.Update(auction);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
