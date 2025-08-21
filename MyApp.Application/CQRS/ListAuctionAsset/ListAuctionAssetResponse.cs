namespace MyApp.Application.CQRS.ListAuctionAsset
{
    public class ListAuctionAssetResponse
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalAuctionAsset { get; set; }
        public List<AuctionAssetResponse> AuctionAssetResponses { get; set; }
        public Dictionary<string, int> CategoryCounts { get; set; }
    }

    public class AuctionAssetResponse
    {
        public Guid AuctionAssetsId { get; set; }
        public string TagName { get; set; }
        public decimal StartingPrice { get; set; }
        public string Unit { get; set; }
        public decimal Deposit { get; set; }
        public decimal RegistrationFee { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime UpdatedAt { get; set; }
        public Guid UpdatedBy { get; set; }
        public Guid AuctionId { get; set; }
        public string AuctionName { get; set; }
        public string CategoryName { get; set; }
    }
}
