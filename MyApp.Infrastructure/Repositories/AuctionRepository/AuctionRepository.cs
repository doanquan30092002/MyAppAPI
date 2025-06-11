using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MyApp.Application.Common.Services.UploadFile;
using MyApp.Application.CQRS.Auction.AddAuction.Commands;
using MyApp.Application.Interfaces.IAuctionRepository;
using MyApp.Core.Entities;
using MyApp.Infrastructure.Data;

namespace MyApp.Infrastructure.Repositories.AuctionRepository
{
    public class AuctionRepository : IAuctionRepository
    {
        private readonly AppDbContext _context;
        private readonly IUploadFile _uploadFileService;

        public AuctionRepository(AppDbContext context, IUploadFile uploadFileService)
        {
            _context = context;
            _uploadFileService = uploadFileService;
        }

        public async Task<Guid> AddAuctionAsync(AddAuctionCommand command, Guid userId)
        {
            string? auctionRulesUrl = null;
            string? auctionPlanningMapUrl = null;

            if (command.AuctionRulesFile != null && command.AuctionRulesFile.Length > 0)
            {
                auctionRulesUrl = await _uploadFileService.UploadAsync(command.AuctionRulesFile);
            }

            // Xử lý upload file bản đồ quy hoạch mới
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
                Auction_Map = command.Auction_Map,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = userId,
                UpdatedAt = DateTime.UtcNow,
                UpdatedBy = userId,
                QRLink = "Test",
                NumberRoundMax = command.NumberRoundMax,
                Status = command.Status,
                WinnerData = command.WinnerData,
                CategoryId = command.CategoryId,
            };

            await _context.Auctions.AddAsync(auction);

            return auction.AuctionId;
        }
    }
}
