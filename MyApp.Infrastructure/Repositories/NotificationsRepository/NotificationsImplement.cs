using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyApp.Application.Interfaces.INofiticationsRepository;
using MyApp.Core.Entities;
using MyApp.Infrastructure.Data;

namespace MyApp.Infrastructure.Repositories.NotificationsRepository
{
    public class NotificationsImplement : INotificationRepository
    {
        private readonly AppDbContext _context;

        public NotificationsImplement(AppDbContext context)
        {
            _context = context;
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
