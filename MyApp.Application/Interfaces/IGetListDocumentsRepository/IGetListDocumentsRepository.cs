using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyApp.Application.CQRS.Auction.GetListAuction.Querries;
using MyApp.Application.CQRS.Auction.GetListAution.Querries;

namespace MyApp.Application.Interfaces.IGetListDocumentsRepository
{
    public interface IGetListDocumentsRepository
    {
        public Task<GetListAuctionDocumentsResponse> GetListAuctionDocumentsAsync(
            GetListAuctionDocumentsRequest getListAuctionDocumentsRequest
        );
    }
}
