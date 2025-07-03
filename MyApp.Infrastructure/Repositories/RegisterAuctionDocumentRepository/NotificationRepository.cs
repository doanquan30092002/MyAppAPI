using MyApp.Application.Interfaces.RegisterAuctionDocument.Repository;
using MyApp.Core.Entities;
using MyApp.Infrastructure.Data;

namespace MyApp.Infrastructure.Repositories.RegisterAuctionDocumentRepository
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly AppDbContext _context;

        public NotificationRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Guid> CreateNotificationAsync(string userId, string message, int type)
        {
            var notification = new Notification
            {
                NotificationId = Guid.NewGuid(),
                UserId = Guid.Parse(userId),
                Message = message,
                NotificationType = type,
                SentAt = DateTime.UtcNow,
                IsRead = false,
                UpdatedAt = DateTime.UtcNow,
                UrlAction = null, // Assuming UrlAction is optional and can be null
            };

            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();

            return notification.NotificationId;
        }
    }
}
