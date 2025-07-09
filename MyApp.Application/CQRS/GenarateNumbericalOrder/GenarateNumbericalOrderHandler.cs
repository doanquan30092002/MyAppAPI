using MediatR;
using MyApp.Application.Interfaces.GenarateNumbericalOrder;

namespace MyApp.Application.CQRS.GenarateNumbericalOrder
{
    public class GenarateNumbericalOrderHandler
        : IRequestHandler<GenarateNumbericalOrderRequest, bool>
    {
        private readonly IGenarateNumbericalOrderRepository _genarateNumbericalOrderRepository;

        public GenarateNumbericalOrderHandler(
            IGenarateNumbericalOrderRepository genarateNumbericalOrderRepository
        )
        {
            _genarateNumbericalOrderRepository = genarateNumbericalOrderRepository;
        }

        public async Task<bool> Handle(
            GenarateNumbericalOrderRequest request,
            CancellationToken cancellationToken
        )
        {
            var result = await _genarateNumbericalOrderRepository.GenerateNumbericalOrderAsync(
                request.AuctionId
            );
            return result;
        }
    }
}
