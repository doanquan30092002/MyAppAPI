using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyApp.Core.Entities;

namespace MyApp.Application.CQRS.GetListEnteredPrices.Querries
{
    public class GetListEnteredPricesResponse
    {
        public List<AuctionRoundPrices> ListAuctionRoundPrices { get; set; }
    }
}
