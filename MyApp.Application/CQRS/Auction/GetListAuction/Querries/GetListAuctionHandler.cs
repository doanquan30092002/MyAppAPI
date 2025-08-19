using MediatR;
using MyApp.Application.Common.CurrentUserService;
using MyApp.Application.CQRS.Auction.GetListAution.Querries;
using MyApp.Application.Interfaces.IGetListRepository;

namespace MyApp.Application.CQRS.Auction.GetListAuction.Querries
{
    public class GetListAuctionHandler
        : IRequestHandler<GetListAuctionRequest, GetListAuctionResponse>
    {
        private readonly IGetListRepository _getListRepository;
        private readonly ICurrentUserService _currentUserService;

        public GetListAuctionHandler(
            IGetListRepository getListRepository,
            ICurrentUserService currentUserService
        )
        {
            _getListRepository =
                getListRepository ?? throw new ArgumentNullException(nameof(getListRepository));
            _currentUserService = currentUserService;
        }

        public Task<GetListAuctionResponse> Handle(
            GetListAuctionRequest request,
            CancellationToken cancellationToken
        )
        {
            string userId = _currentUserService.GetUserId() ?? string.Empty;
            string role = _currentUserService.GetRole() ?? string.Empty;
            if (role != null && role.Equals("Auctioneer"))
            {
                return _getListRepository.GetListAuctionsAsync(request, userId);
            }
            return _getListRepository.GetListAuctionsAsync(request);
        }
    }
}
