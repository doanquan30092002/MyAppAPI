using MyApp.Application.CQRS.ListAuctionRegisted;

namespace MyApp.Application.Interfaces.ListAuctionRegisted
{
    public interface IAuctionRegistedRepository
    {
        Task<AuctionRegistedResponse> ListAuctionRegisted(
            string? userId,
            AuctionRegistedRequest request
        );
    }
}
