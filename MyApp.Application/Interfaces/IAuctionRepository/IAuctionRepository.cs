using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyApp.Application.CQRS.Auction.AddAuction.Commands;
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
        Task<UpdateAuctionResult> UpdateAuctionAsync(UpdateAuctionCommand command, Guid userId);

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
    }
}
