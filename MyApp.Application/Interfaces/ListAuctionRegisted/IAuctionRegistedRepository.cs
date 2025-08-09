using MyApp.Application.CQRS.ListAuctionRegisted;
using MyApp.Core.Entities;

namespace MyApp.Application.Interfaces.ListAuctionRegisted
{
    public interface IAuctionRegistedRepository
    {
        Task<List<Guid>> GetRegisteredAssetIdsAsync(Guid userId);
        Task<List<Guid>> GetRegisteredAuctionIdsAsync(List<Guid> assetIds);
        Task<List<AuctionResponse>> GetAuctionsByIdsAsync(List<Guid> auctionIds);
        Task<List<AuctionAsset>> GetAuctionAssetsByAuctionIdAsync(
            Guid auctionId,
            List<Guid> assetIds
        );
    }
}
