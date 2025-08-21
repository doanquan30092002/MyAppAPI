using MediatR;

namespace MyApp.Application.CQRS.ChangeStatusAuctionRound
{
    public class ChangeStatusAuctionRoundRequest : IRequest<bool>
    {
        public Guid AuctionRoundId { get; set; }
        public int Status { get; set; }
    }
}
