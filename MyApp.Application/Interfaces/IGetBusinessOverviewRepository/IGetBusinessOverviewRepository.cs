using MyApp.Application.CQRS.GetAuctionRoundStatistics.Queries;
using MyApp.Application.CQRS.GetBusinessOverview.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyApp.Application.Interfaces.IGetBusinessOverviewRepository
{
    public interface IGetBusinessOverviewRepository
    {
        Task<GetBusinessOverviewResponse> GetBusinessOverview(
            GetBusinessOverviewRequest request
        );
    }
}
