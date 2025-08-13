using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace MyApp.Application.CQRS.GetBusinessOverview.Queries
{
    public class GetBusinessOverviewRequest : IRequest<GetBusinessOverviewResponse>
    {
        public int? CategoryId { get; set; }

        public DateTime AuctionStartDate { get; set; }

        public DateTime AuctionEndDate { get; set; }
    }
}
