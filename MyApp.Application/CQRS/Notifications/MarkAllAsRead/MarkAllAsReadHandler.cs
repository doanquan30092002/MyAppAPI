using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using MyApp.Application.Interfaces.INotificationsRepository;

namespace MyApp.Application.CQRS.Notifications.MarkAllAsRead
{
    public class MarkAllAsReadHandler : IRequestHandler<MarkAllAsReadRequest, int>
    {
        private readonly INotificationsRepository _notificationsRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public MarkAllAsReadHandler(
            INotificationsRepository notificationsRepository,
            IHttpContextAccessor httpContextAccessor
        )
        {
            _notificationsRepository = notificationsRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<int> Handle(
            MarkAllAsReadRequest request,
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

            // Đánh dấu tất cả notification của user này đã đọc, trả về số lượng cập nhật
            return await _notificationsRepository.MarkAllAsReadAsync(userId);
        }
    }
}
