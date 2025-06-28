using MyApp.Application.CQRS.RegisterAuctionDocument.Command;

namespace MyApp.Application.Interfaces.RegisterAuctionDocument.Repository
{
    public interface IRegisterAuctionDocumentRepository
    {
        Task<bool> CheckAuctionDocumentExsit(string? userId, string auctionAssetsId);
        Task<RegisterAuctionDocumentResponse> CreateQRForPayTicket(Guid auctionDocumentsId);
        Task<Guid> InsertAuctionDocumentAsync(
            string auctionAssetsId,
            string? userId,
            string? bankAccount,
            string? bankAccountNumber,
            string? bankBranch
        );
        Task<string?> UpdateStatusTicketAndGetUserIdAsync(Guid auctionDocumentsId);
    }
}
