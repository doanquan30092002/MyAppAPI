using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using MyApp.Application.Common.CurrentUserService;
using MyApp.Application.Interfaces.INotificationsRepository;

namespace MyApp.Application.CQRS.Notifications.MarkAsRead.Commands
{
    public class MarkAsReadHandler : IRequestHandler<MarkAsReadRequest, bool>
    {
        private readonly INotificationsRepository _notificationsRepository;
        private readonly ICurrentUserService _currentUserService;

        public MarkAsReadHandler(
            INotificationsRepository notificationsRepository,
            ICurrentUserService currentUserService
        )
        {
            _notificationsRepository = notificationsRepository;
            _currentUserService = currentUserService;
        }

        public async Task<bool> Handle(
            MarkAsReadRequest request,
            CancellationToken cancellationToken
        )
        {
            var userIdStr = _currentUserService.GetUserId();

            if (string.IsNullOrEmpty(userIdStr) || !Guid.TryParse(userIdStr, out var userId))
                throw new UnauthorizedAccessException("Không thể lấy UserId từ người dùng.");

            var notification = await _notificationsRepository.GetNotificationByIdAsync(
                request.NotificationId
            );
            if (notification == null)
                throw new KeyNotFoundException("Thông báo không tồn tại.");

            if (notification.UserId != userId)
                throw new UnauthorizedAccessException(
                    "Bạn không có quyền thao tác với thông báo này."
                );

            // Đánh dấu đã đọc
            return await _notificationsRepository.MarkAsReadAsync(request.NotificationId);
        }
    }
}
