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
    }
}
