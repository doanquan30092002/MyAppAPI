using MediatR;

namespace MyApp.Application.CQRS.GenarateNumbericalOrder
{
    public class GenarateNumbericalOrderRequest : IRequest<bool>
    {
        public Guid AuctionId { get; set; }
    }
}
