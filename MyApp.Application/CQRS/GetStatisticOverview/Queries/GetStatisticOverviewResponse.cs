using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyApp.Application.CQRS.GetStatisticOverview.Queries
{
    public class GetStatisticOverviewResponse
    {
        public decimal TotalRevenue { get; set; }
        public int SuccessfulAuctions { get; set; }
        public int TotalParticipants { get; set; }
    }
}
