using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using MyApp.Application.Interfaces.INotificationsRepository;

namespace MyApp.Application.CQRS.Notifications.HasUnread.Command
{
    public class HasUnreadHandler : IRequestHandler<HasUnreadCommand, bool>
    {
        private readonly INotificationsRepository _notificationsRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public HasUnreadHandler(
            INotificationsRepository notificationsRepository,
            IHttpContextAccessor httpContextAccessor
        )
        {
            _notificationsRepository = notificationsRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<bool> Handle(
            HasUnreadCommand request,
            CancellationToken cancellationToken
        )
        {
            // Lấy userId từ Claims
            var userIdStr = _httpContextAccessor
                .HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)
                ?.Value;
            if (!Guid.TryParse(userIdStr, out var userId))
            {
                throw new UnauthorizedAccessException("Không xác định được người dùng.");
            }

            // Kiểm tra xem user có notification nào chưa đọc không
            return await _notificationsRepository.HasUnreadNotificationAsync(userId);
        }
    }
}
