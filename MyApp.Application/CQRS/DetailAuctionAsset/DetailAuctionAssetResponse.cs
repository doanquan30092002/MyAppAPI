namespace MyApp.Application.CQRS.DetailAuctionAsset
{
    public class DetailAuctionAssetResponse
    {
        public AuctionAssetResponse AuctionAssetResponse { get; set; }
        public int TotalAuctionDocument { get; set; }
        public decimal TotalRegistrationFee { get; set; }
        public decimal TotalDeposit { get; set; }
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
        public string AuctionName { get; set; }
    }
}
