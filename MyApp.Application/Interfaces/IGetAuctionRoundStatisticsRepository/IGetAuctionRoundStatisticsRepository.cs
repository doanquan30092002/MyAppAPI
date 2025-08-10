using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyApp.Application.CQRS.GetAuctionRoundStatistics.Queries;

namespace MyApp.Application.Interfaces.IGetAuctionRoundStatisticsRepository
{
    public interface IGetAuctionRoundStatisticsRepository
    {
        Task<GetAuctionRoundStatisticsResponse> GetAuctionRoundStatistics(
            GetAuctionRoundStatisticsRequest request
        );
    }
}
