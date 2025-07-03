namespace MyApp.Application.Interfaces.RegisterAuctionDocument.Service
{
    public interface INotificationService
    {
        Task NotifyUserAsync(string userId, string message, int type);
    }
}
