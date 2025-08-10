using MediatR;

namespace MyApp.Application.CQRS.ListAuctionAsset
{
    public class ListAuctionAssetRequest : IRequest<ListAuctionAssetResponse>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public SearchAuctionAsset Search { get; set; }
    }

    public class SearchAuctionAsset
    {
        public int? CategoryId { get; set; }
        public string? TagName { get; set; }
        public DateTime? AuctionStartDate { get; set; }
        public DateTime? AuctionEndDate { get; set; }
        public int? AuctionStatus { get; set; }
    }
}
