using MediatR;
using MyApp.Application.Interfaces.AuctionDocumentRegisted;

namespace MyApp.Application.CQRS.AuctionDocumentRegisted
{
    public class AuctionDocumentRegistedHandler
        : IRequestHandler<AuctionDocumentRegistedRequest, List<AuctionDocumentRegistedResponse>?>
    {
        private readonly IAuctionDocumentRegistedRepository _repository;

        public AuctionDocumentRegistedHandler(IAuctionDocumentRegistedRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<AuctionDocumentRegistedResponse>?> Handle(
            AuctionDocumentRegistedRequest request,
            CancellationToken cancellationToken
        )
        {
            List<AuctionDocumentRegistedResponse>? auctionDocuments =
                await _repository.GetAuctionDocumentRegistedByAuctionId(
                    request.UserId,
                    request.AuctionId
                );
            return auctionDocuments;
        }
    }
}
