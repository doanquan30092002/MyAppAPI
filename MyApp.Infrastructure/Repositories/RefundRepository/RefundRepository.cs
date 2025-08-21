using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MyApp.Application.CQRS.AuctionDocuments.ConfirmReufund;
using MyApp.Application.Interfaces.IRefundRepository;
using MyApp.Core.Entities;
using MyApp.Infrastructure.Data;

namespace MyApp.Infrastructure.Repositories.RefundRepository
{
    public class RefundRepository : IRefundRepository
    {
        private readonly AppDbContext _context;

        public RefundRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> ConfirmRefundAsync(ConfirmRefundCommand request)
        {
            var documents = await _context
                .Set<AuctionDocuments>()
                .Where(x => request.AuctionDocumentIds.Contains(x.AuctionDocumentsId))
                .ToListAsync();

            if (documents.Count != request.AuctionDocumentIds.Count)
                return false;

            foreach (var doc in documents)
            {
                if (request.StatusTicket.HasValue)
                    doc.StatusTicket = request.StatusTicket.Value;

                if (request.StatusDeposit.HasValue)
                    doc.StatusDeposit = request.StatusDeposit.Value;

                doc.UpdateAtTicket = DateTime.Now;
            }

            return true;
        }

        public async Task<List<AuctionDocuments>> GetAuctionDocumentsByIdsAsync(
            List<Guid> auctionDocumentIds
        )
        {
            if (auctionDocumentIds == null || !auctionDocumentIds.Any())
                return new List<AuctionDocuments>();

            return await _context
                .AuctionDocuments.Where(x => auctionDocumentIds.Contains(x.AuctionDocumentsId))
                .ToListAsync();
        }
    }
}
