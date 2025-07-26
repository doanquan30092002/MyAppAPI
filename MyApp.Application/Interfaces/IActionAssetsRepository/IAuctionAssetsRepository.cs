using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyApp.Core.DTOs.AuctionAssetsDTO;

namespace MyApp.Application.Interfaces.IActionAssetsRepository
{
    public interface IAuctionAssetsRepository
    {
        Task DeleteByAuctionIdAsync(Guid auctionId);

        Task<
            List<AuctionAssetsWithHighestBidResponse>
        > GetAuctionAssetsWithHighestBidByAuctionIdAsync(Guid auctionId);
    }
}
