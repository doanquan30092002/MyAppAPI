using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MyApp.Application.Interfaces.INotificationsRepository;
using MyApp.Core.Entities;
using MyApp.Infrastructure.Data;

namespace MyApp.Infrastructure.Repositories.NotificationsRepository
{
    public class NotificationsImplement : INotificationsRepository
    {
        private readonly AppDbContext _context;

        public NotificationsImplement(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Notification?> GetNotificationByIdAsync(Guid notificationId)
        {
            return await _context.Notifications.FirstOrDefaultAsync(n =>
                n.NotificationId == notificationId
            );
        }

        public async Task<List<Notification>> GetNotificationsByUserIdAsync(
            Guid userId,
            int pageIndex,
            int pageSize
        )
        {
            return await _context
                .Notifications.Where(n => n.UserId == userId)
                .OrderByDescending(n => n.SentAt)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<int> GetTotalNotificationsByUserIdAsync(Guid userId)
        {
            return await _context.Notifications.CountAsync(n => n.UserId == userId);
        }

        public async Task<bool> HasUnreadNotificationAsync(Guid userId)
        {
            return await _context.Notifications.AnyAsync(n => n.UserId == userId && !n.IsRead);
        }

        public async Task<int> MarkAllAsReadAsync(Guid userId)
        {
            var notifications = await _context
                .Notifications.Where(n => n.UserId == userId && !n.IsRead)
                .ToListAsync();

            if (notifications.Count == 0)
                return 0;

            foreach (var n in notifications)
            {
                n.IsRead = true;
                n.UpdatedAt = DateTime.Now;
            }

            _context.Notifications.UpdateRange(notifications);
            return await _context.SaveChangesAsync();
        }

        public async Task<bool> MarkAsReadAsync(Guid notificationId)
        {
            var notification = await _context.Notifications.FirstOrDefaultAsync(n =>
                n.NotificationId == notificationId
            );
            if (notification == null)
                return false;

            notification.IsRead = true;
            notification.UpdatedAt = DateTime.Now;
            _context.Notifications.Update(notification);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> SaveNotificationsAsync(List<Notification> notifications)
        {
            try
            {
                await _context.Notifications.AddRangeAsync(notifications);

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
