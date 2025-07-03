namespace MyApp.Application.Interfaces.RegisterAuctionDocument.Sender
{
    public interface INotificationSender
    {
        Task SendToUserAsync(string userId, object message);
    }
}
