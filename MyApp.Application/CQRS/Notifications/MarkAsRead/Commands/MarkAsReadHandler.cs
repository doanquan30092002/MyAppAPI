using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using MyApp.Application.Interfaces.INotificationsRepository;

namespace MyApp.Application.CQRS.Notifications.MarkAsRead.Commands
{
    public class MarkAsReadHandler : IRequestHandler<MarkAsReadRequest, bool>
    {
        private readonly INotificationsRepository _notificationsRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public MarkAsReadHandler(
            INotificationsRepository notificationsRepository,
            IHttpContextAccessor httpContextAccessor
        )
        {
            _notificationsRepository = notificationsRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<bool> Handle(
            MarkAsReadRequest request,
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

            var notification = await _notificationsRepository.GetNotificationByIdAsync(
                request.NotificationId
            );
            if (notification == null)
                throw new KeyNotFoundException("Notification không tồn tại.");

            if (notification.UserId != userId)
                throw new UnauthorizedAccessException(
                    "Bạn không có quyền thao tác với thông báo này."
                );

            // Đánh dấu đã đọc
            return await _notificationsRepository.MarkAsReadAsync(request.NotificationId);
        }
    }
}
