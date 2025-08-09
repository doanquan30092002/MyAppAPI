using MyApp.Application.CQRS.ListAuctionAsset;

namespace MyApp.Application.Interfaces.ListAuctionAsset
{
    public interface IListAuctionAssetRepository
    {
        Task<List<AuctionAssetResponse>> GetAllAuctionAssets(
            int pageNumber,
            int pageSize,
            SearchAuctionAsset search
        );
        Task<Dictionary<string, int>> GetCategoryCounts();
        Task<int> GetTotalAuctionAssetsCount();
    }
}
