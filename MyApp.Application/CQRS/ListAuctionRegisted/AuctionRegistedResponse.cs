namespace MyApp.Application.CQRS.ListAuctionRegisted
{
    public class AuctionRegistedResponse
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalAuctionRegisted { get; set; }
        public List<AuctionAndAuctionAssetResponse>? AuctionResponse { get; set; }
    }

    public class AuctionResponse
    {
        public Guid AuctionId { get; set; }
        public string AuctionName { get; set; }
        public string CategoryName { get; set; }
        public string AuctionDescription { get; set; }
        public string AuctionRules { get; set; }
        public string? AuctionPlanningMap { get; set; }
        public DateTime RegisterOpenDate { get; set; }
        public DateTime RegisterEndDate { get; set; }
        public DateTime AuctionStartDate { get; set; }
        public DateTime AuctionEndDate { get; set; }
        public int NumberRoundMax { get; set; }

        //0: bản nháp, 1: công khai, 2:Hoàn thành, 3:Hủy
        public int Status { get; set; }
    }

    public class AuctionAndAuctionAssetResponse
    {
        public Guid AuctionId { get; set; }
        public string AuctionName { get; set; }
        public string CategoryName { get; set; }
        public string AuctionDescription { get; set; }
        public string AuctionRules { get; set; }
        public string? AuctionPlanningMap { get; set; }
        public DateTime RegisterOpenDate { get; set; }
        public DateTime RegisterEndDate { get; set; }
        public DateTime AuctionStartDate { get; set; }
        public DateTime AuctionEndDate { get; set; }
        public int NumberRoundMax { get; set; }

        //0: bản nháp, 1: công khai, 2:Hoàn thành, 3:Hủy
        public int Status { get; set; }
        public List<AuctionAsset> AuctionAssets { get; set; } = new List<AuctionAsset>();
    }

    public class AuctionAsset
    {
        public Guid AuctionAssetsId { get; set; }
        public string TagName { get; set; }
        public decimal StartingPrice { get; set; }
        public string Unit { get; set; }
        public decimal Deposit { get; set; }
        public decimal RegistrationFee { get; set; }
        public string Description { get; set; }
        public Guid AuctionId { get; set; }
    }
}
