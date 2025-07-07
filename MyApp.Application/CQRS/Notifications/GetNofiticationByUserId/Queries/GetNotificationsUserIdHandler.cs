using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using MyApp.Application.CQRS.Notifications.GetNotificationByUserId.Queries;
using MyApp.Application.Interfaces.INotificationsRepository;

namespace MyApp.Application.CQRS.Notifications.GetNofiticationByUserId.Queries
{
    public class GetNotificationsUserIdHandler
        : IRequestHandler<GetNotificationsByUserIdRequest, GetNotificationsByUserIdResponse>
    {
        private readonly INotificationsRepository _notificationsRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GetNotificationsUserIdHandler(
            INotificationsRepository notificationsRepository,
            IHttpContextAccessor httpContextAccessor
        )
        {
            _notificationsRepository = notificationsRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<GetNotificationsByUserIdResponse> Handle(
            GetNotificationsByUserIdRequest request,
            CancellationToken cancellationToken
        )
        {
            Guid? userId = null;

            var userIdStr = _httpContextAccessor
                .HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)
                ?.Value;

            if (Guid.TryParse(userIdStr, out var parsedGuid))
            {
                userId = parsedGuid;
            }

            if (userId == null)
                throw new UnauthorizedAccessException("Vui lòng đăng nhập lại để xem thông báo.");

            var notifications = await _notificationsRepository.GetNotificationsByUserIdAsync(
                userId.Value,
                request.PageIndex,
                request.PageSize
            );

            var totalCount = await _notificationsRepository.GetTotalNotificationsByUserIdAsync(
                userId.Value
            );

            var notificationDtos = notifications
                .Select(n => new NotificationDto
                {
                    NotificationId = n.NotificationId,
                    UserId = n.UserId,
                    Message = n.Message,
                    NotificationType = n.NotificationType,
                    SentAt = n.SentAt,
                    IsRead = n.IsRead,
                    UpdatedAt = n.UpdatedAt,
                    UrlAction = n.UrlAction,
                })
                .ToList();

            return new GetNotificationsByUserIdResponse
            {
                TotalCount = totalCount,
                PageIndex = request.PageIndex,
                PageSize = request.PageSize,
                Notifications = notificationDtos,
            };
        }
    }
}
