using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyApp.Core.Entities;

namespace MyApp.Application.Interfaces.INotificationsRepository
{
    public interface INotificationsRepository
    {
        /// <summary>
        /// Lưu danh sách Notification vào database.
        /// </summary>
        /// <param name="notifications">Danh sách Notification cần lưu.</param>
        /// <returns>True nếu lưu thành công, false nếu có lỗi.</returns>
        Task<bool> SaveNotificationsAsync(List<Notification> notifications);

        /// <summary>
        /// Lấy danh sách Notification theo UserId với phân trang.
        /// </summary>
        /// <param name="userId">Guid của người dùng.</param>
        /// <param name="pageIndex">Trang hiện tại (bắt đầu từ 1).</param>
        /// <param name="pageSize">Số lượng bản ghi mỗi trang.</param>
        /// <returns>Danh sách Notification tương ứng.</returns>
        Task<List<Notification>> GetNotificationsByUserIdAsync(
            Guid userId,
            int pageIndex,
            int pageSize
        );

        /// <summary>
        /// Lấy tổng số Notification theo UserId.
        /// </summary>
        /// <param name="userId">Guid của người dùng.</param>
        /// <returns>Tổng số Notification của người dùng.</returns>
        Task<int> GetTotalNotificationsByUserIdAsync(Guid userId);

        /// <summary>
        /// Lấy thông báo theo NotificationId.
        /// </summary>
        Task<Notification?> GetNotificationByIdAsync(Guid notificationId);

        /// <summary>
        /// Kiểm tra người dùng có thông báo mới chưa đọc không.
        /// </summary>
        Task<bool> HasUnreadNotificationAsync(Guid userId);

        /// <summary>
        /// Đánh dấu đã đọc cho 1 notification theo Id.
        /// </summary>
        Task<bool> MarkAsReadAsync(Guid notificationId);

        /// <summary>
        /// Đánh dấu đã đọc tất cả notification chưa đọc của user.
        /// </summary>
        Task<int> MarkAllAsReadAsync(Guid userId);
    }
}
