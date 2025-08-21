using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyApp.Application.CQRS.GetAuctionRoundStatistics.Queries
{
    public class GetAuctionRoundStatisticsResponse
    {
        public int TotalParticipants { get; set; }
        public int TotalAssets { get; set; }
        public int TotalBids { get; set; }
    }
}
