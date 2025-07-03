using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
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

        public async Task<bool> ConfirmRefundAsync(List<Guid> auctionDocumentIds)
        {
            var documents = await _context
                .Set<AuctionDocuments>()
                .Where(x => auctionDocumentIds.Contains(x.AuctionDocumentsId))
                .ToListAsync();

            if (documents.Count != auctionDocumentIds.Count)
                return false;

            foreach (var doc in documents)
            {
                doc.StatusTicket = 3; // 3: đã hoàn tiền hồ sơ
                doc.StatusDeposit = 2; // 2: đã hoàn tiền cọc
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
