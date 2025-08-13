using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace MyApp.Application.CQRS.GetStatisticOverview.Queries
{
    public class GetStatisticOverviewRequest : IRequest<GetStatisticOverviewResponse>
    {
        public int? Month { get; set; }
        public int? Year { get; set; }
    }
}
