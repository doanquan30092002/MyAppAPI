using MediatR;
using MyApp.Application.CQRS.GetAuctionRoundStatistics.Queries;
using MyApp.Application.Interfaces.IGetAuctionRoundStatisticsRepository;
using MyApp.Application.Interfaces.IGetBusinessOverviewRepository;
using MyApp.Application.Interfaces.IGetListAssetInfostatisticsRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyApp.Application.CQRS.GetBusinessOverview.Queries
{
    public class GetBusinessOverviewHandler : IRequestHandler<GetBusinessOverviewRequest, GetBusinessOverviewResponse>
    {
        private readonly IGetBusinessOverviewRepository _getBusinessOverviewRepository;

        public GetBusinessOverviewHandler(
            IGetBusinessOverviewRepository getBusinessOverviewRepository
        )
        {
            _getBusinessOverviewRepository =
                getBusinessOverviewRepository
                ?? throw new ArgumentNullException(nameof(getBusinessOverviewRepository));
        }
        public Task<GetBusinessOverviewResponse> Handle(GetBusinessOverviewRequest request, CancellationToken cancellationToken)
        {
            return _getBusinessOverviewRepository.GetBusinessOverview(request);
        }
    }
}
