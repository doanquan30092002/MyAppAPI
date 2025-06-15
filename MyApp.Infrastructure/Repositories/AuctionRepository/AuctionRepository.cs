using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MyApp.Application.Common.Services.UploadFile;
using MyApp.Application.CQRS.Auction.AddAuction.Commands;
using MyApp.Application.CQRS.Auction.UpdateAuction.Commands;
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

        public async Task<Auction?> FindAuctionByIdAsync(Guid auctionId)
        {
            return await _context.Auctions.FirstOrDefaultAsync(a => a.AuctionId == auctionId);
        }

        public async Task<UpdateAuctionResult> UpdateAuctionAsync(
            UpdateAuctionCommand command,
            Guid userId
        )
        {
            var auction = await _context.Auctions.FirstOrDefaultAsync(a =>
                a.AuctionId == command.AuctionId
            );
            if (auction == null)
                throw new ValidationException("Auction không tồn tại.");

            int oldStatus = auction.Status;
            int newStatus = command.Status;

            if (newStatus == 1 && DateTime.Now > auction.AuctionEndDate)
                throw new ValidationException(
                    "Không thể công khai phiên đấu giá do ngày kết thúc phiên đấu giá đã qua."
                );

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

            return new UpdateAuctionResult
            {
                AuctionId = auction.AuctionId,
                StatusChangedToTrue = (oldStatus == 0 && newStatus == 1),
                AuctionEndDate = auction.AuctionEndDate,
            };
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
    }
}
