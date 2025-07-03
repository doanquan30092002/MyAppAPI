namespace MyApp.Application.CQRS.Auction.GetListAuction.Querries
{
    public class GetListAuctionDocumentsResponse
    {
        public int TotalCount { get; set; }

        public List<ListAuctionDocumentsDTO> AuctionDocuments { get; set; } =
            new List<ListAuctionDocumentsDTO>();
    }

    public class ListAuctionDocumentsDTO
    {
        public Guid AuctionDocumentsId { get; set; }
        public string CitizenIdentification { get; set; }
        public string Name { get; set; }

        public string TagName { get; set; }

        public decimal Deposit { get; set; }

        public int StatusDeposit { get; set; }

        public decimal RegistrationFee { get; set; }

        public int StatusTicket { get; set; }

        public int? NumericalOrder { get; set; }

        public string? Note { get; set; }
    }
}
