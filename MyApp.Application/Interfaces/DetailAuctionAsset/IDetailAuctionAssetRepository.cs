using MyApp.Application.CQRS.DetailAuctionAsset;

namespace MyApp.Application.Interfaces.DetailAuctionAsset
{
    public interface IDetailAuctionAssetRepository
    {
        Task<AuctionAssetResponse?> GetAuctionAssetByIdAsync(Guid auctionAssetsId);
        Task<int> GetTotalAuctionDocumentsAsync(Guid auctionAssetsId);
        Task<decimal> GetTotalDepositAsync(Guid auctionAssetsId);
        Task<decimal> GetTotalRegistrationFeeAsync(Guid auctionAssetsId);
    }
}
