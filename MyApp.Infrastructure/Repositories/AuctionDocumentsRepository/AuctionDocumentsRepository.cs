using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MyApp.Application.Interfaces.IAuctionDocuments;
using MyApp.Core.DTOs.AuctionDocumentsDTO;
using MyApp.Core.Entities;
using MyApp.Infrastructure.Data;

namespace MyApp.Infrastructure.Repositories.AuctionDocumentsRepository
{
    public class AuctionDocumentsRepository : IAuctionDocuments
    {
        private readonly AppDbContext _context;

        public AuctionDocumentsRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<AuctionDocumentDto>> GetAllDocumentsByAuctionIdAsync(Guid auctionId)
        {
            var query =
                from doc in _context.AuctionDocuments
                join asset in _context.AuctionAssets
                    on doc.AuctionAssetId equals asset.AuctionAssetsId
                join auction in _context.Auctions on asset.AuctionId equals auction.AuctionId
                join user in _context.Users on doc.UserId equals user.Id
                join acc in _context.Accounts on user.Id equals acc.UserId
                where auction.AuctionId == auctionId
                select new AuctionDocumentDto
                {
                    AuctionDocumentsId = doc.AuctionDocumentsId,
                    CreateAtTicket = doc.CreateAtTicket,
                    AssetName = asset.TagName,
                    UserName = user.Name,
                    RecentLocation = user.RecentLocation,
                    CitizenIdentification = user.CitizenIdentification,
                    PhoneNumber = acc.PhoneNumber,
                    Deposit = asset.Deposit,
                    Result = null,
                };

            return await query.ToListAsync();
        }

        public async Task<Auction> GetAuctionByAuctionDocumentIdAsync(Guid auctionDocumentId)
        {
            var document = await _context
                .AuctionDocuments.Include(d => d.AuctionAsset)
                .ThenInclude(aa => aa.Auction)
                .FirstOrDefaultAsync(d => d.AuctionDocumentsId == auctionDocumentId);

            if (document == null)
            {
                throw new KeyNotFoundException(
                    $"Auction document with ID {auctionDocumentId} not found."
                );
            }

            if (document.AuctionAsset?.Auction == null)
            {
                throw new KeyNotFoundException(
                    $"Auction not found for document ID {auctionDocumentId}."
                );
            }

            return document.AuctionAsset.Auction;
        }

        public async Task<AuctionDocuments?> GetDocumentByIdAndUserIdAsync(
            Guid auctionDocumentId,
            Guid userId
        )
        {
            return await _context
                .AuctionDocuments.Include(d => d.User)
                .Include(d => d.AuctionAsset)
                .FirstOrDefaultAsync(d =>
                    d.AuctionDocumentsId == auctionDocumentId && d.UserId == userId
                );
        }

        public async Task<AuctionDocuments> GetDocumentByIdAsync(Guid auctionDocumentId)
        {
            var document = await _context
                .AuctionDocuments.Include(d => d.User)
                .Include(d => d.AuctionAsset)
                .FirstOrDefaultAsync(d => d.AuctionDocumentsId == auctionDocumentId);

            if (document == null)
            {
                throw new KeyNotFoundException($"Hồ sơ ID {auctionDocumentId} không tồn tại.");
            }

            return document;
        }

        public async Task<bool> RequestRefundAsync(
            List<Guid> auctionDocumentIds,
            Guid userId,
            string refundProofUrl,
            string refundReason
        )
        {
            var documents = await _context
                .AuctionDocuments.Where(d =>
                    auctionDocumentIds.Contains(d.AuctionDocumentsId) && d.UserId == userId
                )
                .ToListAsync();

            if (!documents.Any())
            {
                return false;
            }

            foreach (var document in documents)
            {
                //if (document.StatusDeposit == 1 && document.StatusRefund == null)
                //{
                //    document.StatusRefund = 1;
                //    document.RefundReason = refundReason;
                //    document.RefundProof = refundProofUrl;
                //    document.UpdateAtTicket = DateTime.Now;
                //    document.NoteReviewRefund = null;
                //}

                if (document.StatusRefund == null)
                {
                    document.StatusRefund = 1;
                    document.RefundReason = refundReason;
                    document.RefundProof = refundProofUrl;
                    document.UpdateAtTicket = DateTime.Now;
                    document.NoteReviewRefund = null;
                }
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ReviewRequestRefundAsync(
            List<Guid> auctionDocumentIds,
            int statusRefund,
            string? noteReviewRefund
        )
        {
            var documents = await _context
                .AuctionDocuments.Where(d => auctionDocumentIds.Contains(d.AuctionDocumentsId))
                .ToListAsync();

            if (!documents.Any())
            {
                return false;
            }

            foreach (var document in documents)
            {
                // Only allow review if StatusRefund is 1 (Refund requested)
                if (document.StatusRefund == 1 || document.StatusRefund == 3)
                {
                    document.StatusRefund = statusRefund; // 2 (Accepted) or 3 (Rejected)
                    document.NoteReviewRefund = noteReviewRefund;
                    document.UpdateAtTicket = DateTime.Now;
                    document.NoteReviewRefund = noteReviewRefund;
                }
            }

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> UpdateIsAttendedAsync(
            List<Guid> auctionDocumentIds,
            bool isAttended
        )
        {
            var documents = await _context
                .AuctionDocuments.Where(d => auctionDocumentIds.Contains(d.AuctionDocumentsId))
                .ToListAsync();

            if (!documents.Any())
            {
                return false;
            }

            foreach (var document in documents)
            {
                document.IsAttended = isAttended;
                document.UpdateAtTicket = DateTime.Now;
            }

            await _context.SaveChangesAsync();
            return true;
        }
    }
}
