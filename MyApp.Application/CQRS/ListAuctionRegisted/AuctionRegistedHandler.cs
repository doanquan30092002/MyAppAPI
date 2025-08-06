using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Http;
using MyApp.Application.Interfaces.ListAuctionRegisted;

namespace MyApp.Application.CQRS.ListAuctionRegisted
{
    public class AuctionRegistedHandler
        : IRequestHandler<AuctionRegistedRequest, AuctionRegistedResponse>
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

        public async Task<AuctionRegistedResponse> Handle(
            AuctionRegistedRequest request,
            CancellationToken cancellationToken
        )
        {
            string userId = _httpContextAccessor
                .HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)
                ?.Value;
            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedAccessException("User not authenticated.");
            AuctionRegistedResponse response = await _repository.ListAuctionRegisted(
                userId,
                request
            );
            return response;
        }
    }
}
