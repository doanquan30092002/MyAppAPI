using MediatR;
using MyApp.Application.CQRS.Auction.GetListAution.Querries;
using MyApp.Application.Interfaces.IGetListRepository;

namespace MyApp.Application.CQRS.Auction.GetListAuction.Querries
{
    public class GetListAuctionHandler
        : IRequestHandler<GetListAuctionRequest, GetListAuctionResponse>
    {
        private readonly IGetListRepository _getListRepository;

        public GetListAuctionHandler(IGetListRepository getListRepository)
        {
            _getListRepository =
                _getListRepository ?? throw new ArgumentNullException(nameof(getListRepository));
        }

        public Task<GetListAuctionResponse> Handle(
            GetListAuctionRequest request,
            CancellationToken cancellationToken
        )
        {
            return _getListRepository.GetListAuctionsAsync(request);
        }
    }
}
