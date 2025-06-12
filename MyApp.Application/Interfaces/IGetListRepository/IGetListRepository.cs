using MyApp.Application.CQRS.Auction.GetListAuction.Querries;
using MyApp.Application.CQRS.Auction.GetListAution.Querries;

namespace MyApp.Application.Interfaces.IGetListRepository
{
    public interface IGetListRepository
    {
        public Task<GetListAuctionResponse> GetListAuctionsAsync(
            GetListAuctionRequest getListAuctionRequest
        );
    }
}
