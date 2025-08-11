using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyApp.Application.CQRS.GetListAssetInfoStatistic.Queries
{
    public class GetListAssetInfoStatisticsResponse
    {
        public string AssetName { get; set; }
        public bool HasWinner { get; set; }
        public int TotalParticipants { get; set; }
        public int TotalBids { get; set; }
        public decimal HighestPrice { get; set; }
        public decimal StartingPrice { get; set; }
    }
}
