using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Http;
using MyApp.Application.CQRS.Auction.GetListAution.Querries;
using MyApp.Application.Interfaces.IGetListRepository;

namespace MyApp.Application.CQRS.Auction.GetListAuction.Querries
{
    public class GetListAuctionHandler
        : IRequestHandler<GetListAuctionRequest, GetListAuctionResponse>
    {
        private readonly IGetListRepository _getListRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GetListAuctionHandler(
            IGetListRepository getListRepository,
            IHttpContextAccessor httpContextAccessor
        )
        {
            _getListRepository =
                getListRepository ?? throw new ArgumentNullException(nameof(getListRepository));
            _httpContextAccessor =
                httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        public Task<GetListAuctionResponse> Handle(
            GetListAuctionRequest request,
            CancellationToken cancellationToken
        )
        {
            string userId = _httpContextAccessor
                .HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)
                ?.Value;
            string role = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Role)?.Value;
            if (role != null && role.Equals("Auctioneer"))
            {
                return _getListRepository.GetListAuctionsAsync(request, userId);
            }
            return _getListRepository.GetListAuctionsAsync(request);
        }
    }
}
