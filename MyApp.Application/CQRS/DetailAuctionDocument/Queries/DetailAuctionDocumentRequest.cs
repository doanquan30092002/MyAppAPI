using MediatR;

namespace MyApp.Application.CQRS.DetailAuctionDocument.Queries
{
    public class DetailAuctionDocumentRequest : IRequest<DetailAuctionDocumentResponse?>
    {
        public Guid AuctionDocumentsId { get; set; }
    }
}
