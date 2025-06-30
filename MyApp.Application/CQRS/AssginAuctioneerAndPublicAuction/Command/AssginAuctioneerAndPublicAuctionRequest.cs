using MediatR;

namespace MyApp.Application.CQRS.AssginAuctioneerAndPublicAuction.Command
{
    public class AssginAuctioneerAndPublicAuctionRequest
        : IRequest<AssginAuctioneerAndPublicAuctionResponse>
    {
        public Guid AuctionId { get; set; }
        public Guid Auctioneer { get; set; }
    }
}
