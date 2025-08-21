using MediatR;
using MyApp.Application.Interfaces.IGetAuctionByIdRepository;
using MyApp.Application.Interfaces.IGetListRepository;

namespace MyApp.Application.CQRS.Auction.GetListAuctionById.Querries
{
    public class GetAuctionByIdHandler
        : IRequestHandler<GetAuctionByIdRequest, GetAuctionByIdResponse>
    {
        private readonly IGetAuctionByIdRepository _getAuctionByIdRepository;

        public GetAuctionByIdHandler(IGetAuctionByIdRepository getAuctionByIdRepository)
        {
            _getAuctionByIdRepository = getAuctionByIdRepository;
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
