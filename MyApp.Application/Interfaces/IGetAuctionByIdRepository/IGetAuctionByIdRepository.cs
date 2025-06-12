using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyApp.Application.CQRS.Auction.GetListAuctionById.Querries;

namespace MyApp.Application.Interfaces.IGetAuctionByIdRepository
{
    public interface IGetAuctionByIdRepository
    {
        Task<GetAuctionByIdResponse> GetAuctionByIdAsync(Guid auctionId);
    }
}
