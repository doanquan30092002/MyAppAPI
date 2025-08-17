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

namespace MyApp.Application.CQRS.Notifications.HasUnread.Command
{
    public class HasUnreadHandler : IRequestHandler<HasUnreadCommand, bool>
    {
        private readonly INotificationsRepository _notificationsRepository;
        private readonly ICurrentUserService _currentUserService;

        public HasUnreadHandler(
            INotificationsRepository notificationsRepository,
            ICurrentUserService currentUserService
        )
        {
            _notificationsRepository = notificationsRepository;
            _currentUserService = currentUserService;
        }

        public async Task<bool> Handle(
            HasUnreadCommand request,
            CancellationToken cancellationToken
        )
        {
            var userIdStr = _currentUserService.GetUserId();

            if (string.IsNullOrEmpty(userIdStr) || !Guid.TryParse(userIdStr, out var userId))
                throw new UnauthorizedAccessException("Không thể lấy UserId từ người dùng.");

            // Kiểm tra xem user có notification nào chưa đọc không
            return await _notificationsRepository.HasUnreadNotificationAsync(userId);
        }
    }
}
