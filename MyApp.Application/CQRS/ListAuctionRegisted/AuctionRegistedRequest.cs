using MediatR;

namespace MyApp.Application.CQRS.ListAuctionRegisted
{
    public class AuctionRegistedRequest : IRequest<AuctionRegistedResponse>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public Guid UserId { get; set; }
        public SearchAuctionRegisted Search { get; set; }
    }

    public class SearchAuctionRegisted
    {
        public string? AuctionName { get; set; }
        public DateTime? AuctionStartDate { get; set; }
        public DateTime? AuctionEndDate { get; set; }
    }
}
