using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MyApp.Application.CQRS.DetailAuctionDocument.Queries;
using MyApp.Application.Interfaces.DetailAuctionDocument;
using MyApp.Core.Entities;
using MyApp.Infrastructure.Data;

namespace MyApp.Infrastructure.Repositories.DetailAuctionDocument
{
    public class DetailAuctionDocumentRepository : IDetailAuctionDocumentRepository
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public DetailAuctionDocumentRepository(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<DetailAuctionDocumentResponse?> GetDetailAuctionDocumentByAuctionDocumentsIdAsync(
            Guid auctionDocumentsId
        )
        {
            var auctionDocument = await _context.AuctionDocuments.FirstOrDefaultAsync(ad =>
                ad.AuctionDocumentsId == auctionDocumentsId
            );
            if (auctionDocument == null)
            {
                return null; // or throw an exception if preferred
            }
            return new DetailAuctionDocumentResponse()
            {
                AuctionDocumentsId = auctionDocument.AuctionDocumentsId,
                UserId = auctionDocument.UserId,
                AuctionAssetId = auctionDocument.AuctionAssetId,
                BankAccount = auctionDocument.BankAccount,
                BankAccountNumber = auctionDocument.BankAccountNumber,
                BankBranch = auctionDocument.BankBranch,
                CreateByTicket = auctionDocument.CreateByTicket,
                CreateAtTicket = auctionDocument.CreateAtTicket,
                UpdateAtTicket = auctionDocument.UpdateAtTicket,
                CreateAtDeposit = auctionDocument.CreateAtDeposit,
                StatusTicket = auctionDocument.StatusTicket,
                StatusDeposit = auctionDocument.StatusDeposit,
                StatusRefundDeposit = auctionDocument.StatusRefundDeposit,
                NumericalOrder = auctionDocument.NumericalOrder,
                Note = auctionDocument.Note,
            };
        }
    }
}
