using MyApp.Application.CQRS.RegisterAuctionDocument.Command;
using MyApp.Core.Entities;

namespace MyApp.Application.Interfaces.RegisterAuctionDocument.Repository
{
    public interface IRegisterAuctionDocumentRepository
    {
        Task<AuctionDocumentResponse?> CheckAuctionDocumentPaid(
            string? userId,
            string auctionAssetsId
        );
        Task<RegisterAuctionDocumentResponse> CreateQRForPayTicket(Guid auctionDocumentsId);
        Task<string> GetAuctionNameByAuctionDocumentsIdAsync(Guid? auctionDocumentsId);
        Task<List<Guid>> GetUserIdByRoleAsync();
        Task<Guid> InsertAuctionDocumentAsync(
            string auctionAssetsId,
            string? userId,
            string? bankAccount,
            string? bankAccountNumber,
            string? bankBranch
        );
        Task SaveNotificationAsync(List<Guid> userIdStaff, string message);
        Task<bool> UpdateInforBankFromUser(
            Guid auctionDocumentsId,
            string? bankAccount,
            string? bankAccountNumber,
            string? bankBranch
        );
        Task<bool> UpdateStatusTicketAndGetUserIdAsync(Guid? auctionDocumentsId);
    }
}
