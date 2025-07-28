using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyApp.Core.Entities;

namespace MyApp.Application.CQRS.GetListAuctionRound.Querries
{
    public class GetListAuctionRoundResponse
    {
        public List<AuctionRound> AuctionRounds { get; set; } = new List<AuctionRound>();
    }
}
