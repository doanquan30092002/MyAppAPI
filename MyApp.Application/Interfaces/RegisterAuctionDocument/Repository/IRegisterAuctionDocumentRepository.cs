using MyApp.Application.CQRS.RegisterAuctionDocument.Command;
using MyApp.Core.Entities;

namespace MyApp.Application.Interfaces.RegisterAuctionDocument.Repository
{
    public interface IRegisterAuctionDocumentRepository
    {
        Task<AuctionDocuments?> CheckAuctionDocumentPaid(string? userId, string auctionAssetsId);
        Task<RegisterAuctionDocumentResponse> CreateQRForPayTicket(Guid auctionDocumentsId);
        Task<Guid> InsertAuctionDocumentAsync(
            string auctionAssetsId,
            string? userId,
            string? bankAccount,
            string? bankAccountNumber,
            string? bankBranch
        );
        Task<bool> UpdateInforBankFromUser(
            Guid auctionDocumentsId,
            string? bankAccount,
            string? bankAccountNumber,
            string? bankBranch
        );
        Task<bool> UpdateStatusTicketAndGetUserIdAsync(Guid? auctionDocumentsId);
    }
}
