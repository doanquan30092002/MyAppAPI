using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyApp.Core.DTOs.AuctionDocumentsDTO;
using MyApp.Core.Entities;

namespace MyApp.Application.Interfaces.IAuctionDocuments
{
    public interface IAuctionDocuments
    {
        Task<List<AuctionDocumentDto>> GetAllDocumentsByAuctionIdAsync(Guid auctionId);

        Task<AuctionDocuments> GetDocumentByIdAsync(Guid auctionDocumentId);

        Task<bool> UpdateIsAttendedAsync(List<Guid> auctionDocumentIds, bool isAttended);

        Task<Auction> GetAuctionByAuctionDocumentIdAsync(Guid auctionDocumentId);

        Task<AuctionDocuments?> GetDocumentByIdAndUserIdAsync(Guid auctionDocumentId, Guid userId);

        Task<bool> RequestRefundAsync(
            List<Guid> auctionDocumentIds,
            Guid userId,
            string refundProofUrl,
            string refundReason
        );

        Task<bool> ReviewRequestRefundAsync(
            List<Guid> auctionDocumentIds,
            int statusRefund,
            string? noteReviewRefund
        );
    }
}
