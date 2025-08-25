using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyApp.Application.CQRS.GetListBidders.Queries
{
    public class GetListBiddersResponse
    {
        public List<BidderDto> ListBidders { get; set; } = new List<BidderDto>();
    }

    public class BidderDto
    {
        public Guid UserId { get; set; }
        public bool IsBidPlaced { get; set; }
    }
}
