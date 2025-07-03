namespace MyApp.Application.Interfaces.RegisterAuctionDocument.Repository
{
    public interface INotificationRepository
    {
        Task<Guid> CreateNotificationAsync(string userId, string message, int type);
    }
}
