using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyApp.Application.CQRS.AuctionDocuments.FindHighestPriceAndFlag.Queries;

namespace MyApp.Application.Interfaces.IFindHighestPriceAndFlag
{
    public interface IFindHighestPriceAndFlag
    {
        /// <summary>
        /// Tìm ra giá cao nhất và flag cho một auction, dựa vào auctionId và userId.
        /// </summary>
        /// <param name="auctionId">Guid của phiên đấu giá</param>
        /// <param name="userId">Guid của người dùng</param>
        /// <returns>
        /// Trả về thông tin giá cao nhất và trạng thái flag (FindHighestPriceAndFlagResponse)
        /// </returns>
        Task<FindHighestPriceAndFlagResponse> FindHighestPriceAndFlag(Guid auctionId, Guid userId);

        /// <summary>
        /// Lấy toàn bộ giá và trạng thái flag của tất cả người dùng trong một phiên đấu giá.
        /// </summary>
        /// <param name="auctionId">Guid của phiên đấu giá</param>
        /// <returns>
        /// Trả về toàn bộ thông tin giá cao nhất và flag theo từng người dùng và asset.
        /// </returns>
        Task<FindHighestPriceAndFlagResponse> GetAllHighestPriceAndFlagByAuctionId(Guid auctionId);
    }
}
