namespace MyApp.Application.Interfaces.ReceiveAuctionRegistrationForm
{
    public interface IReceiveAuctionRegistrationFormRepository
    {
        Task<string> GetAuctionNameByAuctionDocumentsIdAsync(Guid auctionDocumentsId);
        Task<List<Guid>> GetUserIdByAuctionDocumentId(Guid auctionDocumentsId);
        Task SaveNotificationAsync(List<Guid> userId, string message);
        Task<bool> UpdateStatusTicketSigned(
            Guid auctionDocumentsId,
            int statusTicket,
            string? note
        );
    }
}
