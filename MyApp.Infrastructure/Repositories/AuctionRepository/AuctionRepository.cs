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
            var uploadTasks = new List<Task<string>>();
            var fileMapping = new Dictionary<int, string>();
            int index = 0;

            // AuctionRulesFile
            if (command.AuctionRulesFile?.Length > 0)
            {
                uploadTasks.Add(_uploadFileService.UploadAsync(command.AuctionRulesFile));
                fileMapping[index++] = "AuctionRules";
            }

            // AuctionPlanningMap
            if (command.AuctionPlanningMap?.Length > 0)
            {
                uploadTasks.Add(_uploadFileService.UploadAsync(command.AuctionPlanningMap));
                fileMapping[index++] = "AuctionPlanningMap";
            }

            // LegalDocuments
            if (command.LegalDocuments != null && command.LegalDocuments.Count > 0)
            {
                foreach (var file in command.LegalDocuments.Where(f => f.Length > 0))
                {
                    uploadTasks.Add(_uploadFileService.UploadAsync(file));
                    fileMapping[index++] = "LegalDocument";
                }
            }

            var results = await Task.WhenAll(uploadTasks);

            string? auctionRulesUrl = null;
            string? auctionPlanningMapUrl = null;
            var legalDocumentUrls = new List<string>();

            for (int i = 0; i < results.Length; i++)
            {
                var url = results[i];
                if (fileMapping[i] == "AuctionRules")
                    auctionRulesUrl = url;
                else if (fileMapping[i] == "AuctionPlanningMap")
                    auctionPlanningMapUrl = url;
                else if (fileMapping[i] == "LegalDocument" && !string.IsNullOrEmpty(url))
                    legalDocumentUrls.Add(url!);
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
                QRLink = "",
                NumberRoundMax = command.NumberRoundMax,
                Status = 0,
                WinnerData = null,
                CategoryId = command.CategoryId,
                Updateable = true,
                CancelReasonFile = "No file uploaded",
                CancelReason = null,
                legalDocumentUrls = legalDocumentUrls.Any()
                    ? Newtonsoft.Json.JsonConvert.SerializeObject(legalDocumentUrls)
                    : null,
                PriceMin = command.PriceMin,
                PriceMax = command.PriceMax,
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

            // Check công khai
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

            // Nếu đã công khai rồi
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
                // Gom tất cả upload file vào 1 list
                var uploadTasks = new List<Task<string>>();
                var fileMapping = new Dictionary<int, string>();
                int index = 0;

                if (command.AuctionRulesFile?.Length > 0)
                {
                    uploadTasks.Add(_uploadFileService.UploadAsync(command.AuctionRulesFile));
                    fileMapping[index++] = "AuctionRules";
                }

                if (command.AuctionPlanningMap?.Length > 0)
                {
                    uploadTasks.Add(_uploadFileService.UploadAsync(command.AuctionPlanningMap));
                    fileMapping[index++] = "AuctionPlanningMap";
                }

                if (command.LegalDocuments != null && command.LegalDocuments.Count > 0)
                {
                    foreach (var file in command.LegalDocuments.Where(f => f.Length > 0))
                    {
                        uploadTasks.Add(_uploadFileService.UploadAsync(file));
                        fileMapping[index++] = "LegalDocument";
                    }
                }

                // Chạy tất cả upload song song
                var results = uploadTasks.Any()
                    ? await Task.WhenAll(uploadTasks)
                    : Array.Empty<string>();

                var legalDocumentUrls = new List<string>();

                for (int i = 0; i < results.Length; i++)
                {
                    var url = results[i];
                    if (fileMapping[i] == "AuctionRules")
                        auction.AuctionRules = url;
                    else if (fileMapping[i] == "AuctionPlanningMap")
                        auction.AuctionPlanningMap = url;
                    else if (fileMapping[i] == "LegalDocument" && !string.IsNullOrEmpty(url))
                        legalDocumentUrls.Add(url);
                }

                if (!uploadTasks.Any(f => fileMapping.ContainsValue("AuctionPlanningMap")))
                {
                    auction.AuctionPlanningMap = "No file uploaded";
                }

                if (legalDocumentUrls.Any())
                {
                    auction.legalDocumentUrls = Newtonsoft.Json.JsonConvert.SerializeObject(
                        legalDocumentUrls
                    );
                }

                // Update các field khác
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
                auction.PriceMin = command.PriceMin;
                auction.PriceMax = command.PriceMax;
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
            // Lấy thông tin auction
            var auction = await _context.Auctions.FirstOrDefaultAsync(a =>
                a.AuctionId == auctionId
            );
            if (auction == null)
                throw new KeyNotFoundException($"Phiên đấu giá với id {auctionId} không tồn tại.");

            // Danh sách người thắng (winner)
            var auctionAssetsWithWinners =
                await _auctionAssetsRepository.GetAuctionAssetsWithHighestBidByAuctionIdAsync(
                    auctionId
                );

            var excludedPairs = auctionAssetsWithWinners
                .Where(a =>
                    a.HighestBid != null
                    && !string.IsNullOrWhiteSpace(a.HighestBid.CitizenIdentification)
                )
                .Select(a => new
                {
                    AuctionAssetId = a.AuctionAssetsId,
                    CitizenId = a.HighestBid.CitizenIdentification.Trim(),
                })
                .ToList();

            // Lấy danh sách AuctionAssetId thuộc auction
            var auctionAssetIds = await _context
                .AuctionAssets.Where(x => x.AuctionId == auctionId)
                .Select(x => x.AuctionAssetsId)
                .ToListAsync();

            IQueryable<AuctionDocuments> query = _context
                .AuctionDocuments.Where(doc => auctionAssetIds.Contains(doc.AuctionAssetId))
                .Include(doc => doc.User)
                .Include(doc => doc.AuctionAsset);

            switch (auction.Status)
            {
                case 3: // Hủy
                    query = query.Where(doc => doc.StatusTicket == 1 || doc.StatusDeposit == 1);
                    break;

                case 2: // Hoàn thành
                    query = query.Where(doc =>
                        doc.IsAttended == true || (doc.IsAttended == false && doc.StatusRefund == 2)
                    );
                    break;

                default:
                    return new List<AuctionDocuments>();
            }

            // Thực thi query trên DB trước
            var result = await query.ToListAsync();

            // Nếu hoàn thành thì loại bỏ người thắng ở bộ nhớ
            if (auction.Status == 2)
            {
                result = result
                    .Where(doc =>
                        !excludedPairs.Any(p =>
                            p.AuctionAssetId == doc.AuctionAssetId
                            && p.CitizenId == doc.User.CitizenIdentification
                        )
                    )
                    .ToList();
            }

            return result;
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

        public async Task<bool> WaitingPublicAsync(Guid auctionId, Guid managerInCharge)
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

            var account = await _context
                .Accounts.Include(a => a.Role)
                .FirstOrDefaultAsync(a => a.UserId == managerInCharge);

            if (account == null)
                throw new KeyNotFoundException("Không tìm thấy người quản lý phụ trách.");

            if (account.RoleId != 6) // role manager
                throw new ValidationException(
                    "Người phụ trách không có quyền quản lý (RoleId phải là 6)."
                );

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

            auction.ManagerInCharge = account.UserId.ToString();
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
