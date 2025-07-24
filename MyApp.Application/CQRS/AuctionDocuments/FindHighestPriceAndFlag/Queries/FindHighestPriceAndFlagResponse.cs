using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyApp.Application.CQRS.AuctionDocuments.FindHighestPriceAndFlag.Queries
{
    public class FindHighestPriceAndFlagResponse
    {
        public Dictionary<Guid, List<PriceFlagDto>> Data { get; set; }
    }
}

public class PriceFlagDto
{
    public decimal Price { get; set; }
    public bool Flag { get; set; }
}
