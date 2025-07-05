using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using MyApp.Application.CQRS.Notifications.GetNofiticationByUserId.Queries;
using MyApp.Application.Interfaces.INotificationsRepository;

namespace MyApp.Application.CQRS.Notifications.GetNotificationById.Queries
{
    public class GetNotificationByIdHandler
        : IRequestHandler<GetNotificationsByIdRequest, NotificationDto>
    {
        private readonly INotificationsRepository _notificationsRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GetNotificationByIdHandler(
            INotificationsRepository notificationsRepository,
            IHttpContextAccessor httpContextAccessor
        )
        {
            _notificationsRepository = notificationsRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<NotificationDto> Handle(
            GetNotificationsByIdRequest request,
            CancellationToken cancellationToken
        )
        {
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
            {
                throw new KeyNotFoundException("Thông báo không tồn tại.");
            }

            if (notification.UserId != userId)
            {
                throw new UnauthorizedAccessException("Bạn không có quyền truy cập thông báo này.");
            }

            return new NotificationDto
            {
                NotificationId = notification.NotificationId,
                UserId = notification.UserId,
                Message = notification.Message,
                NotificationType = notification.NotificationType,
                SentAt = notification.SentAt,
                IsRead = notification.IsRead,
                UpdatedAt = notification.UpdatedAt,
                UrlAction = notification.UrlAction,
            };
        }
    }
}
