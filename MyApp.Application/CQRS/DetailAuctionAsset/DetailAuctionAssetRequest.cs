using MediatR;

namespace MyApp.Application.CQRS.DetailAuctionAsset
{
    public class DetailAuctionAssetRequest : IRequest<DetailAuctionAssetResponse>
    {
        public Guid AuctionAssetsId { get; set; }
    }
}
