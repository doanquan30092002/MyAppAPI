using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Http;
using MyApp.Application.Interfaces.AuctionDocumentRegisted;

namespace MyApp.Application.CQRS.AuctionDocumentRegisted
{
    public class AuctionDocumentRegistedHandler
        : IRequestHandler<AuctionDocumentRegistedRequest, List<AuctionDocumentRegistedResponse>?>
    {
        private readonly IAuctionDocumentRegistedRepository _repository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuctionDocumentRegistedHandler(
            IAuctionDocumentRegistedRepository repository,
            IHttpContextAccessor httpContextAccessor
        )
        {
            _repository = repository;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<List<AuctionDocumentRegistedResponse>?> Handle(
            AuctionDocumentRegistedRequest request,
            CancellationToken cancellationToken
        )
        {
            string userId = _httpContextAccessor
                .HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)
                ?.Value;
            List<AuctionDocumentRegistedResponse>? auctionDocuments =
                await _repository.GetAuctionDocumentRegistedByAuctionId(userId, request.AuctionId);
            return auctionDocuments;
        }
    }
}
