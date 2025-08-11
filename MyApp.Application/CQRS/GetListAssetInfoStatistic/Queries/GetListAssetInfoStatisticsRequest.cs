using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using MyApp.Application.CQRS.GetAuctionRoundStatistics.Queries;

namespace MyApp.Application.CQRS.GetListAssetInfoStatistic.Queries
{
    public class GetListAssetInfostatisticsRequest : IRequest<GetListAssetInfoStatisticsResponse>
    {
        public Guid AuctionAssetsId { get; set; }
    }
}
