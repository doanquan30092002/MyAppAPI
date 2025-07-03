using MyApp.Application.Interfaces.RegisterAuctionDocument.Repository;
using MyApp.Application.Interfaces.RegisterAuctionDocument.Sender;
using MyApp.Application.Interfaces.RegisterAuctionDocument.Service;

namespace MyApp.Infrastructure.Services.RegisterAuctionDocument.Service
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _repository;
        private readonly INotificationSender _sender;

        public NotificationService(INotificationRepository repository, INotificationSender sender)
        {
            _repository = repository;
            _sender = sender;
        }

        public async Task NotifyUserAsync(string userId, string message, int type)
        {
            var notificationId = await _repository.CreateNotificationAsync(userId, message, type);

            await _sender.SendToUserAsync(
                userId,
                new
                {
                    NotificationId = notificationId,
                    Message = message,
                    Type = type,
                    SentAt = DateTime.UtcNow,
                }
            );
        }
    }
}
