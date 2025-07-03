using Microsoft.AspNetCore.SignalR;
using MyApp.Application.Interfaces.RegisterAuctionDocument.Sender;
using MyApp.Infrastructure.Services.Realtime;

namespace MyApp.Infrastructure.Services.RegisterAuctionDocument.Sender
{
    public class NotificationSender : INotificationSender
    {
        private readonly IHubContext<NotificationHub> _hubContext;

        public NotificationSender(IHubContext<NotificationHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task SendToUserAsync(string userId, object message)
        {
            await _hubContext.Clients.User(userId).SendAsync("ReceiveNotification", message);
        }
    }
}
