using MediatR;
using MyApp.Application.Interfaces.IGetAuctionByIdRepository;
using MyApp.Application.Interfaces.IGetListRepository;

namespace MyApp.Application.CQRS.Auction.GetListAuctionById.Querries
{
    public class GetAuctionByIdHandler
        : IRequestHandler<GetAuctionByIdRequest, GetAuctionByIdResponse>
    {
        private readonly IGetAuctionByIdRepository _getAuctionByIdRepository;

        public GetAuctionByIdHandler(IGetListRepository getListRepository)
        {
            _getAuctionByIdRepository =
                _getAuctionByIdRepository
                ?? throw new ArgumentNullException(nameof(getListRepository));
        }

        public async Task<GetAuctionByIdResponse> Handle(
            GetAuctionByIdRequest request,
            CancellationToken cancellationToken
        )
        {
            return await _getAuctionByIdRepository.GetAuctionByIdAsync(request.AuctionId);
        }
    }
}
