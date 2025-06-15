using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace MyApp.Application.CQRS.Auction.GetListAuctionById.Querries
{
    public class GetAuctionByIdRequest : IRequest<GetAuctionByIdResponse>
    {
        public Guid AuctionId { get; set; }
    }
}
