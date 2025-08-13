using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using MyApp.Application.CQRS.GetAuctionRoundStatistics.Queries;
using MyApp.Application.Interfaces.IGetAuctionRoundStatisticsRepository;
using MyApp.Application.Interfaces.IGetStatisticOverviewRepository;

namespace MyApp.Application.CQRS.GetStatisticOverview.Queries
{
    public class GetStatisticOverviewHandler
        : IRequestHandler<GetStatisticOverviewRequest, GetStatisticOverviewResponse>
    {
        private readonly IGetStatisticOverviewRepository _getStatisticOverviewRepository;

        public GetStatisticOverviewHandler(
            IGetStatisticOverviewRepository getStatisticOverviewRepository
        )
        {
            _getStatisticOverviewRepository =
                getStatisticOverviewRepository
                ?? throw new ArgumentNullException(nameof(getStatisticOverviewRepository));
        }

        public Task<GetStatisticOverviewResponse> Handle(
            GetStatisticOverviewRequest request,
            CancellationToken cancellationToken
        )
        {
            return _getStatisticOverviewRepository.GetStatisticOverview(request);
        }
    }
}
