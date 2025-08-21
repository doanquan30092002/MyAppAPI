using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using MyApp.Application.CQRS.GetAuctionRoundStatistics.Queries;
using MyApp.Application.Interfaces.IGetAuctionRoundStatisticsRepository;
using MyApp.Application.Interfaces.IGetListAssetInfostatisticsRepository;

namespace MyApp.Application.CQRS.GetListAssetInfoStatistic.Queries
{
    public class GetListAssetInfoStatisticsHandler
        : IRequestHandler<GetListAssetInfostatisticsRequest, GetListAssetInfoStatisticsResponse>
    {
        private readonly IGetListAssetInfostatisticsRepository _getListAssetInfostatisticsRepository;

        public GetListAssetInfoStatisticsHandler(
            IGetListAssetInfostatisticsRepository getListAssetInfostatisticsRepository
        )
        {
            _getListAssetInfostatisticsRepository = getListAssetInfostatisticsRepository;
        }

        public Task<GetListAssetInfoStatisticsResponse> Handle(
            GetListAssetInfostatisticsRequest request,
            CancellationToken cancellationToken
        )
        {
            return _getListAssetInfostatisticsRepository.GetAuctionAssetsStatistics(request);
        }
    }
}
