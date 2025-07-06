using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using MyApp.Application.Common.Services.NotificationHub;
using MyApp.Application.Common.Services.SendMessage;
using MyApp.Application.Common.Services.UploadFile;
using MyApp.Application.CQRS.Auction.UpdateAuction.Commands;
using MyApp.Application.Interfaces.IAuctionRepository;
using MyApp.Application.Interfaces.INotificationsRepository;
using MyApp.Application.Interfaces.IUnitOfWork;
using MyApp.Core.Entities;

namespace MyApp.Application.CQRS.Auction.CancelAuction.Commands
{
    public class CancelAuctionHandler : IRequestHandler<CancelAuctionCommand, bool>
    {
        private readonly IAuctionRepository _auctionRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEnumerable<ISendMessage> _sendMessages;
        private readonly INotificationsRepository _notificationRepository;
        private readonly INotificationSender _notificationSender;

        public CancelAuctionHandler(
            IAuctionRepository auctionRepository,
            IHttpContextAccessor httpContextAccessor,
            IUnitOfWork unitOfWork,
            IEnumerable<ISendMessage> sendMessages,
            INotificationsRepository notificationRepository,
            INotificationSender notificationSender
        )
        {
            _auctionRepository = auctionRepository;
            _httpContextAccessor = httpContextAccessor;
            _unitOfWork = unitOfWork;
            _sendMessages = sendMessages;
            _notificationRepository = notificationRepository;
            _notificationSender = notificationSender;
        }

        public async Task<bool> Handle(
            CancelAuctionCommand request,
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
                throw new UnauthorizedAccessException("Không thể lấy UserId từ người dùng.");

            try
            {
                // 1. Bắt đầu transaction
                _unitOfWork.BeginTransaction();

                // 2. Hủy đấu giá
                await _auctionRepository.CancelAuctionAsync(request, userId.Value);

                // 3. Lấy danh sách hồ sơ đã đăng ký/cọc
                var listDocs = await _auctionRepository.GetPaidOrDepositedDocumentsByAuctionIdAsync(
                    request.AuctionId
                );

                // 4. Lấy danh sách userId
                var userIds = listDocs.Select(x => x.UserId).Distinct().ToList();

                // 5. Lấy danh sách email từ userId
                var emailList = await _auctionRepository.GetEmailsByUserIdsAsync(userIds);

                // 6. Gửi thông báo nội bộ (notification)
                var notifications = new List<Notification>();
                foreach (var uid in userIds)
                {
                    notifications.Add(
                        new Notification
                        {
                            NotificationId = Guid.NewGuid(),
                            UserId = uid,
                            Message =
                                $"Phiên đấu giá bạn đã đăng ký hoặc đặt cọc đã bị hủy. Lý do: {request.CancelReason}",
                            NotificationType = 2,
                            SentAt = DateTime.Now,
                            IsRead = false,
                            UpdatedAt = DateTime.Now,
                            UrlAction = null,
                        }
                    );
                }
                if (notifications.Any())
                {
                    await _notificationRepository.SaveNotificationsAsync(notifications);
                }

                await _unitOfWork.CommitAsync();

                // send email
                var emailSender = _sendMessages.FirstOrDefault(x =>
                    x.Channel == SendMessageChannel.Email
                );

                if (emailSender != null && emailList.Any())
                {
                    var subject = "Thông báo hủy phiên đấu giá";
                    var content =
                        $"Phiên đấu giá bạn đã đăng ký hoặc đặt cọc đã bị hủy. Lý do: {request.CancelReason}";
                    await emailSender.SendAsync("", subject, content, emailList);
                }

                // send signalR
                await _notificationSender.SendToUsersAsync(
                    userIds,
                    new
                    {
                        Title = "Thông báo từ phiên đấu giá",
                        Content = "Hủy phiên đấu giá " + request.AuctionId,
                        Time = DateTime.UtcNow,
                    }
                );

                return true;
            }
            catch (Exception)
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }
    }
}
