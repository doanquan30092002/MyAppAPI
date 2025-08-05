namespace MyApp.Application.CQRS.Auction.GetListAuction.Querries
{
    public class GetListAuctionResponse
    {
        public int TotalCount { get; set; }

        public List<ListAuctionDTO> Auctions { get; set; } = new List<ListAuctionDTO>();
    }

    public class ListAuctionDTO
    {
        public Guid AuctionId { get; set; }
        public string AuctionName { get; set; }
        public int CategoryId { get; set; }

        public int Status { get; set; }
        public string? CancelReasonFile { get; set; }

        public string? CancelReason { get; set; }

        public string? RejectReason { get; set; }
        public DateTime RegisterOpenDate { get; set; }
        public DateTime RegisterEndDate { get; set; }
        public DateTime AuctionStartDate { get; set; }
        public DateTime AuctionEndDate { get; set; }
        public string CreatedByUserName { get; set; }
        public string UpdateByUserName { get; set; }
        public string AuctioneerBy { get; set; }
    }
}
