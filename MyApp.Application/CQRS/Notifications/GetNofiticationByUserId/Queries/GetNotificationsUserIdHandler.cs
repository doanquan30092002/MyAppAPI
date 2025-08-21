using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using MyApp.Application.Common.CurrentUserService;
using MyApp.Application.CQRS.Notifications.GetNotificationByUserId.Queries;
using MyApp.Application.Interfaces.INotificationsRepository;

namespace MyApp.Application.CQRS.Notifications.GetNofiticationByUserId.Queries
{
    public class GetNotificationsUserIdHandler
        : IRequestHandler<GetNotificationsByUserIdRequest, GetNotificationsByUserIdResponse>
    {
        private readonly INotificationsRepository _notificationsRepository;
        private readonly ICurrentUserService _currentUserService;

        public GetNotificationsUserIdHandler(
            INotificationsRepository notificationsRepository,
            ICurrentUserService currentUserService
        )
        {
            _notificationsRepository = notificationsRepository;
            _currentUserService = currentUserService;
        }

        public async Task<GetNotificationsByUserIdResponse> Handle(
            GetNotificationsByUserIdRequest request,
            CancellationToken cancellationToken
        )
        {
            var userIdStr = _currentUserService.GetUserId();

            if (string.IsNullOrEmpty(userIdStr) || !Guid.TryParse(userIdStr, out var userId))
                throw new UnauthorizedAccessException("Không thể lấy UserId từ người dùng.");

            var notifications = await _notificationsRepository.GetNotificationsByUserIdAsync(
                userId,
                request.PageIndex,
                request.PageSize
            );

            var totalCount = await _notificationsRepository.GetTotalNotificationsByUserIdAsync(
                userId
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
