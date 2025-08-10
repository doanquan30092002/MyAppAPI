using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using MyApp.Application.CQRS.DetailAuctionDocument.Queries;

namespace MyApp.Application.CQRS.GetAuctionRoundStatistics.Queries
{
    public class GetAuctionRoundStatisticsRequest : IRequest<GetAuctionRoundStatisticsResponse?>
    {
        public Guid AuctionId { get; set; }
    }
}
