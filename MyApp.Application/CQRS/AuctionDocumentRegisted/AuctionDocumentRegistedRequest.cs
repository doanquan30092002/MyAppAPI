using MediatR;

namespace MyApp.Application.CQRS.AuctionDocumentRegisted
{
    public class AuctionDocumentRegistedRequest : IRequest<List<AuctionDocumentRegistedResponse>?>
    {
        public Guid AuctionId { get; set; }
        public Guid UserId { get; set; }
    }
}
