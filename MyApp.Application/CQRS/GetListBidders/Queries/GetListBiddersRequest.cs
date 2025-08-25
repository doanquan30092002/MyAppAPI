using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using MyApp.Application.CQRS.Auction.GetListAuction.Querries;

namespace MyApp.Application.CQRS.GetListBidders.Queries
{
    public class GetListBiddersRequest : IRequest<GetListBiddersResponse>
    {
        public Guid AuctionId { get; set; }
    }
}
