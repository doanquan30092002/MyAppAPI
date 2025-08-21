using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using MyApp.Application.CQRS.AssginAuctioneerAndPublicAuction.Command;
using MyApp.Application.Interfaces.IGetAuctionRoundStatisticsRepository;
using MyApp.Application.Interfaces.IGetListDocumentsRepository;

namespace MyApp.Application.CQRS.GetAuctionRoundStatistics.Queries
{
    public class GetAuctionRoundStatisticsHandler
        : IRequestHandler<GetAuctionRoundStatisticsRequest, GetAuctionRoundStatisticsResponse>
    {
        private readonly IGetAuctionRoundStatisticsRepository _getAuctionRoundStatisticsRepository;

        public GetAuctionRoundStatisticsHandler(
            IGetAuctionRoundStatisticsRepository getAuctionRoundStatisticsRepository
        )
        {
            _getAuctionRoundStatisticsRepository = getAuctionRoundStatisticsRepository;
        }

        public Task<GetAuctionRoundStatisticsResponse> Handle(
            GetAuctionRoundStatisticsRequest request,
            CancellationToken cancellationToken
        )
        {
            return _getAuctionRoundStatisticsRepository.GetAuctionRoundStatistics(request);
        }
    }
}
