using MyApp.Application.CQRS.ListAuctionRegisted;

namespace MyApp.Application.Interfaces.ListAuctionRegisted
{
    public interface IAuctionRegistedRepository
    {
        Task<List<AuctionRegistedResponse>?> ListAuctionRegisted(string? userId);
    }
}
