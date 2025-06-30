using MyApp.Application.Common.Message;

namespace MyApp.Application.CQRS.RegisterAuctionDocument.Command
{
    public class RegisterAuctionDocumentResponse
    {
        public int Code { get; set; }
        public string Message { get; set; }
        public string? QrUrl { get; set; }
        public string? AccountNumber { get; set; }
        public string? BeneficiaryBank { get; set; }
        public decimal? AmountTicket { get; set; }
        public string? Description { get; set; }
    }

    public class RegisterAuctionDocumentResponseDTO
    {
        public string? QrUrl { get; set; }
        public string? AccountNumber { get; set; }
        public string? BeneficiaryBank { get; set; }
        public decimal? AmountTicket { get; set; }
        public string? Description { get; set; }
    }
}
