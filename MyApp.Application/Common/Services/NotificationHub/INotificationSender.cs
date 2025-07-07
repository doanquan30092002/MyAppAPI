using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace MyApp.Application.Common.Services.NotificationHub
{
    public interface INotificationSender
    {
        Task SendToUsersAsync(List<Guid> userIds, object notification);
    }

    public class NotificationSender : INotificationSender
    {
        private readonly IHubContext<NotificationsHub> _hubContext;

        public NotificationSender(IHubContext<NotificationsHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task SendToUsersAsync(List<Guid> userIds, object notification)
        {
            foreach (var userId in userIds)
            {
                await _hubContext
                    .Clients.Group(userId.ToString())
                    .SendAsync("ReceiveNotification", notification);
            }
        }
    }
}
