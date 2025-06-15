using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyApp.Application.CQRS.Auction.UpdateAuction.Commands
{
    public class UpdateAuctionResult
    {
        public Guid AuctionId { get; set; }
        public bool StatusChangedToTrue { get; set; }
        public DateTime AuctionEndDate { get; set; }
    }
}
