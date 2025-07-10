using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Http;
using MyApp.Application.Interfaces.ListAuctionRegisted;

namespace MyApp.Application.CQRS.ListAuctionRegisted
{
    public class AuctionRegistedHandler
        : IRequestHandler<AuctionRegistedRequest, List<AuctionRegistedResponse>?>
    {
        private readonly IAuctionRegistedRepository _repository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuctionRegistedHandler(
            IAuctionRegistedRepository repository,
            IHttpContextAccessor httpContextAccessor
        )
        {
            _repository = repository;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<List<AuctionRegistedResponse?>> Handle(
            AuctionRegistedRequest request,
            CancellationToken cancellationToken
        )
        {
            string userId = _httpContextAccessor
                .HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)
                ?.Value;

            List<AuctionRegistedResponse>? response = await _repository.ListAuctionRegisted(userId);
            return response;
        }
    }
}
