using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyApp.Core.Entities;

namespace MyApp.Core.DTOs.AuctionAssetsDTO
{
    public class AuctionAssetsWithHighestBidResponse : AuctionAssets
    {
        public HighestBidInfo HighestBid { get; set; }
    }
}

public class HighestBidInfo
{
    public decimal Price { get; set; }
    public string CitizenIdentification { get; set; }
    public string Name { get; set; }
}
