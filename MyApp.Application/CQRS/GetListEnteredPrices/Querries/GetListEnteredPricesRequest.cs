using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using MyApp.Application.CQRS.Auction.GetListAuctionById.Querries;

namespace MyApp.Application.CQRS.GetListEnteredPrices.Querries
{
    public class GetListEnteredPricesRequest : IRequest<GetListEnteredPricesResponse>
    {
        public Guid AuctionRoundId { get; set; }
    }
}
