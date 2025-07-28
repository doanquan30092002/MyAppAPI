using MediatR;
using MyApp.Application.Interfaces.ChangeStatusAuctionRound;

namespace MyApp.Application.CQRS.ChangeStatusAuctionRound
{
    public class ChangeStatusAuctionRoundHandler
        : IRequestHandler<ChangeStatusAuctionRoundRequest, bool>
    {
        private readonly IChangeStatusAuctionRoundRepository _repository;

        public ChangeStatusAuctionRoundHandler(IChangeStatusAuctionRoundRepository repository)
        {
            _repository = repository;
        }

        public async Task<bool> Handle(
            ChangeStatusAuctionRoundRequest request,
            CancellationToken cancellationToken
        )
        {
            bool result = await _repository.ChangeStatusAuctionRoundAsync(
                request.AuctionRoundId,
                request.Status
            );
            return result;
        }
    }
}
