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

namespace MyApp.Application.CQRS.Notifications.MarkAllAsRead
{
    public class MarkAllAsReadHandler : IRequestHandler<MarkAllAsReadRequest, int>
    {
        private readonly INotificationsRepository _notificationsRepository;
        private readonly ICurrentUserService _currentUserService;

        public MarkAllAsReadHandler(
            INotificationsRepository notificationsRepository,
            ICurrentUserService currentUserService
        )
        {
            _notificationsRepository = notificationsRepository;
            _currentUserService = currentUserService;
        }

        public async Task<int> Handle(
            MarkAllAsReadRequest request,
            CancellationToken cancellationToken
        )
        {
            var userIdStr = _currentUserService.GetUserId();

            if (string.IsNullOrEmpty(userIdStr) || !Guid.TryParse(userIdStr, out var userId))
                throw new UnauthorizedAccessException("Không thể lấy UserId từ người dùng.");

            // Đánh dấu tất cả notification của user này đã đọc, trả về số lượng cập nhật
            return await _notificationsRepository.MarkAllAsReadAsync(userId);
        }
    }
}
