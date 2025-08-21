using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyApp.Application.CQRS.GetBusinessOverview.Queries
{
    public class GetBusinessOverviewResponse
    {
        public int TotalAuctions { get; set; }
        public int TotalParticipants { get; set; }
        public int TotalSuccessfulAuctions { get; set; }
        public int TotalCancelledAuctions { get; set; }
    }
}
