using Microsoft.EntityFrameworkCore;
using MyApp.Application.CQRS.AuctionDocumentRegisted;
using MyApp.Application.Interfaces.AuctionDocumentRegisted;
using MyApp.Infrastructure.Data;

namespace MyApp.Infrastructure.Repositories.AuctionDocumentRegisted
{
    public class AuctionDocumentRegistedRepository : IAuctionDocumentRegistedRepository
    {
        private readonly AppDbContext _context;

        public AuctionDocumentRegistedRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<AuctionDocumentRegistedResponse>?> GetAuctionDocumentRegistedByAuctionId(
            string? userId,
            Guid auctionId
        )
        {
            var result = await (
                from auctionDocument in _context.AuctionDocuments
                join auctionAsset in _context.AuctionAssets
                    on auctionDocument.AuctionAssetId equals auctionAsset.AuctionAssetsId
                join user in _context.Users on auctionDocument.UserId equals user.Id
                where
                    auctionAsset.AuctionId == auctionId
                    && auctionDocument.UserId.ToString() == userId
                    && auctionDocument.StatusTicket == 2
                    && auctionDocument.StatusDeposit == 1
                select new AuctionDocumentRegistedResponse
                {
                    AuctionDocumentsId = auctionDocument.AuctionDocumentsId,
                    CitizenIdentification = user.CitizenIdentification,
                    Deposit = auctionAsset.Deposit,
                    Name = user.Name,
                    Note = auctionDocument.Note,
                    NumericalOrder = auctionDocument.NumericalOrder,
                    RegistrationFee = auctionAsset.RegistrationFee,
                    StatusDeposit = auctionDocument.StatusDeposit,
                    StatusTicket = auctionDocument.StatusTicket,
                    TagName = auctionAsset.TagName,
                    BankAccount = auctionDocument.BankAccount,
                    BankAccountNumber = auctionDocument.BankAccountNumber,
                    BankBranch = auctionDocument.BankBranch,
                }
            ).ToListAsync();

            return result.Count > 0 ? result : null;
        }
    }
}
