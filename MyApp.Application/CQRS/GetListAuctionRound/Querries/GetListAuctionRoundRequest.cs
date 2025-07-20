using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using MyApp.Application.CQRS.CreateAutionRound.Command;

namespace MyApp.Application.CQRS.GetListAuctionRound.Querries
{
    public class GetListAuctionRoundRequest : IRequest<GetListAuctionRoundResponse>
    {
        public Guid AuctionId { get; set; }
    }
}
