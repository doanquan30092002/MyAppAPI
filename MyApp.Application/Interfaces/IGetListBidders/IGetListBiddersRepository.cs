using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyApp.Application.CQRS.GetListBidders.Queries;

namespace MyApp.Application.Interfaces.IGetListBidders
{
    public interface IGetListBiddersRepository
    {
        Task<GetListBiddersResponse> GetListBidders(GetListBiddersRequest request);
    }
}
