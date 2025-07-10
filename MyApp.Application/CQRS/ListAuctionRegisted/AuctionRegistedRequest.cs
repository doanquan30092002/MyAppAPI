using MediatR;

namespace MyApp.Application.CQRS.ListAuctionRegisted
{
    public class AuctionRegistedRequest : IRequest<List<AuctionRegistedResponse>?> { }
}
