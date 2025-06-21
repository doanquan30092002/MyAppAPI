using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyApp.Core.DTOs.AuctionDocumentsDTO;
using MyApp.Core.Entities;

namespace MyApp.Application.Interfaces.IPaymentDeposit
{
    public interface IPaymentDeposit
    {
        /// <summary>
        /// Lấy thông tin thanh toán theo AuctionDocumentId.
        /// </summary>
        /// <param name="auctionDocumentId">Id hồ sơ đấu giá</param>
        /// <returns>Thông tin thanh toán, null nếu không tồn tại</returns>
        Task<InforPaymentDepositResponse> GetPaymentDepositInfoAsync(Guid auctionDocumentId);

        Task<bool> UpdateStatusDepositAsync(
            Guid auctionDocumentId,
            bool statusDeposit,
            decimal amount
        );
    }
}
