using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyApp.Core.Entities;

namespace MyApp.Application.Interfaces.INofiticationsRepository
{
    public interface INotificationsRepository
    {
        /// <summary>
        /// Lưu danh sách Notification vào database.
        /// </summary>
        /// <param name="notifications">Danh sách Notification cần lưu.</param>
        /// <returns>True nếu lưu thành công, false nếu có lỗi.</returns>
        Task<bool> SaveNotificationsAsync(List<Notification> notifications);
    }
}
