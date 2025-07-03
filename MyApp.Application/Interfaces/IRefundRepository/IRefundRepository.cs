using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyApp.Core.Entities;

namespace MyApp.Application.Interfaces.IRefundRepository
{
    public interface IRefundRepository
    {
        /// <summary>
        /// Xác nhận thanh toán hoàn tiền cho các hồ sơ dựa trên danh sách AuctionDocumentId.
        /// </summary>
        /// <param name="auctionDocumentIds">Danh sách GUID của hồ sơ đấu giá cần xác nhận hoàn tiền.</param>
        /// <returns>True nếu xác nhận thành công, false nếu thất bại.</returns>
        Task<bool> ConfirmRefundAsync(List<Guid> auctionDocumentIds);

        /// <summary>
        /// Lấy danh sách AuctionDocuments theo danh sách GUID.
        /// </summary>
        /// <param name="auctionDocumentIds">Danh sách GUID của hồ sơ đấu giá.</param>
        /// <returns>Danh sách AuctionDocuments tương ứng.</returns>
        Task<List<AuctionDocuments>> GetAuctionDocumentsByIdsAsync(List<Guid> auctionDocumentIds);
    }
}
