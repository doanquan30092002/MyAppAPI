using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyApp.Application.CQRS.Auction.AddAuction.Commands;
using MyApp.Application.CQRS.Auction.CancelAuction.Commands;
using MyApp.Application.CQRS.Auction.UpdateAuction.Commands;
using MyApp.Core.Entities;

namespace MyApp.Application.Interfaces.IAuctionRepository
{
    public interface IAuctionRepository
    {
        Task<Guid> AddAuctionAsync(AddAuctionCommand command, Guid userId);

        /// <summary>
        /// Cập nhật thông tin phiên đấu giá và trả về kết quả cập nhật.
        /// </summary>
        /// <param name="command">Lệnh cập nhật phiên đấu giá.</param>
        /// <param name="userId">Id người thực hiện cập nhật.</param>
        /// <returns>Kết quả cập nhật đấu giá.</returns>
        Task<bool> UpdateAuctionAsync(UpdateAuctionCommand command, Guid userId);

        /// <summary>
        /// Tìm phiên đấu giá theo Id.
        /// </summary>
        /// <param name="auctionId">Id của phiên đấu giá.</param>
        /// <returns>Đối tượng Auction hoặc null nếu không tìm thấy.</returns>
        Task<Auction?> FindAuctionByIdAsync(Guid auctionId);

        /// <summary>
        /// Cập nhật trường Updateable của phiên đấu giá.
        /// </summary>
        /// <param name="auctionId">Id phiên đấu giá.</param>
        /// <param name="updateable">Giá trị Updateable mới.</param>
        /// <returns>Task hoàn thành.</returns>
        Task UpdateAuctionUpdateableAsync(Guid auctionId, bool updateable);

        /// <summary>
        /// Huỷ phiên đấu giá.
        /// </summary>
        /// <param name="command">Lệnh huỷ phiên đấu giá.</param>
        /// <param name="userId">Id người thực hiện huỷ.</param>
        /// <returns>True nếu huỷ thành công, false nếu thất bại.</returns>
        Task<bool> CancelAuctionAsync(CancelAuctionCommand command, Guid userId);

        /// <summary>
        /// Lấy danh sách hồ sơ đã chuyển tiền phiếu đăng ký hồ sơ hoặc đã cọc theo auctionId.
        /// </summary>
        /// <param name="auctionId">Id phiên đấu giá.</param>
        /// <returns>Danh sách AuctionDocuments phù hợp.</returns>
        Task<List<AuctionDocuments>> GetPaidOrDepositedDocumentsByAuctionIdAsync(Guid auctionId);

        Task<List<string>> GetEmailsByUserIdsAsync(List<Guid> userIds);

        /// <summary>
        /// Cập nhật trạng thái phiên đấu giá sang chờ công bố.
        /// </summary>
        /// <param name="auctionId">Id phiên đấu giá.</param>
        /// <returns>Task hoàn thành.</returns>
        Task<bool> WaitingPublicAsync(Guid auctionId, Guid managerInCharge);

        /// <summary>
        /// Từ chối phiên đấu giá ở trạng thái chờ công bố.
        /// </summary>
        /// <param name="auctionId">ID phiên đấu giá.</param>
        /// <param name="rejectReason">Lý do từ chối.</param>
        /// <param name="userId">ID người thực hiện từ chối.</param>
        /// <returns>True nếu thành công, ngược lại ném lỗi.</returns>
        Task<bool> RejectAuctionAsync(Guid auctionId, string rejectReason);

        Task<bool> UpdateStatusAsync(Guid auctionId, int status);

        /// <summary>
        /// Đánh dấu phiên đấu giá là thành công.
        /// </summary>
        /// <param name="auctionId">ID phiên đấu giá.</param>
        /// <returns>True nếu cập nhật thành công, false nếu thất bại.</returns>
        Task<bool> MarkAuctionAsSuccessfulAsync(Guid auctionId);
    }
}
