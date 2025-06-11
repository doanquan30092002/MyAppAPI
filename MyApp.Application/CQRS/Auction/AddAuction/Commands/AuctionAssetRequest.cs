using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyApp.Application.CQRS.Auction.AddAuction.Commands
{
    public class AuctionAssetRequest
    {
        public string TagName { get; set; }
        public decimal StartingPrice { get; set; }
        public string Unit { get; set; }
        public decimal Deposit { get; set; }
        public decimal RegistrationFee { get; set; }
        public string Description { get; set; }
    }
}
