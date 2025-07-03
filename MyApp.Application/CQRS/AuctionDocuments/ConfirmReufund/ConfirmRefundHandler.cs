using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using MyApp.Application.Interfaces.INofiticationsRepository;
using MyApp.Application.Interfaces.IRefundRepository;
using MyApp.Application.Interfaces.IUnitOfWork;
using MyApp.Core.Entities;

namespace MyApp.Application.CQRS.AuctionDocuments.ConfirmReufund
{
    public class ConfirmRefundHandler : IRequestHandler<ConfirmRefundCommand, bool>
    {
        private readonly IRefundRepository _refundRepository;
        private readonly INotificationsRepository _notificationRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ConfirmRefundHandler(
            IRefundRepository refundRepository,
            INotificationsRepository notificationRepository,
            IUnitOfWork unitOfWork
        )
        {
            _refundRepository = refundRepository;
            _notificationRepository = notificationRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(
            ConfirmRefundCommand request,
            CancellationToken cancellationToken
        )
        {
            if (request.AuctionDocumentIds == null || request.AuctionDocumentIds.Count == 0)
                return false;

            try
            {
                // Bắt đầu transaction
                _unitOfWork.BeginTransaction();

                // 1. Xác nhận hoàn tiền
                var result = await _refundRepository.ConfirmRefundAsync(request.AuctionDocumentIds);
                if (!result)
                {
                    await _unitOfWork.RollbackAsync();
                    return false;
                }

                // 2. Lấy danh sách hồ sơ để tạo thông báo
                var documents = await _refundRepository.GetAuctionDocumentsByIdsAsync(
                    request.AuctionDocumentIds
                );

                var notifications = new List<Notification>();
                foreach (var doc in documents)
                {
                    notifications.Add(
                        new Notification
                        {
                            NotificationId = Guid.NewGuid(),
                            UserId = doc.UserId,
                            Message =
                                $"Hồ sơ đấu giá của bạn (ID: {doc.AuctionDocumentsId}) đã được hoàn tiền.",
                            NotificationType = 1,
                            SentAt = DateTime.Now,
                            IsRead = false,
                            UpdatedAt = DateTime.Now,
                            UrlAction = null,
                        }
                    );
                }

                await _notificationRepository.SaveNotificationsAsync(notifications);

                // 3. Commit transaction
                await _unitOfWork.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();

                return false;
            }
        }
    }
}
