using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyApp.Application.CQRS.GetAuctionRoundStatistics.Queries;
using MyApp.Application.CQRS.GetListAssetInfoStatistic.Queries;

namespace MyApp.Application.Interfaces.IGetListAssetInfostatisticsRepository
{
    public interface IGetListAssetInfostatisticsRepository
    {
        Task<GetListAssetInfoStatisticsResponse> GetAuctionAssetsStatistics(
            GetListAssetInfostatisticsRequest request
        );
    }
}
