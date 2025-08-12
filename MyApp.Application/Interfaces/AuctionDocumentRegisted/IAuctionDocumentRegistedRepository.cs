using MyApp.Application.CQRS.AuctionDocumentRegisted;

namespace MyApp.Application.Interfaces.AuctionDocumentRegisted
{
    public interface IAuctionDocumentRegistedRepository
    {
        Task<List<AuctionDocumentRegistedResponse>?> GetAuctionDocumentRegistedByAuctionId(
            Guid userId,
            Guid auctionId
        );
    }
}
