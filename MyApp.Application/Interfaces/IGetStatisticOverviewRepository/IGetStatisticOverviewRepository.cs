using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyApp.Application.CQRS.GetStatisticOverview.Queries;

namespace MyApp.Application.Interfaces.IGetStatisticOverviewRepository
{
    public interface IGetStatisticOverviewRepository
    {
        Task<GetStatisticOverviewResponse> GetStatisticOverview(
            GetStatisticOverviewRequest request
        );
    }
}
