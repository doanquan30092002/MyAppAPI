namespace MyApp.Application.CQRS.AuctionDocumentRegisted
{
    public class AuctionDocumentRegistedResponse
    {
        public Guid AuctionDocumentsId { get; set; }
        public string CitizenIdentification { get; set; }
        public decimal Deposit { get; set; }
        public string Name { get; set; }
        public string? Note { get; set; }
        public int? NumericalOrder { get; set; }
        public decimal RegistrationFee { get; set; }
        public int StatusDeposit { get; set; }
        public int StatusTicket { get; set; }
        public string TagName { get; set; }
    }
}
