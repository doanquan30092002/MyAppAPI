using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyApp.Core.Entities;

namespace MyApp.Application.Interfaces.IGetListEnteredPricesRepository
{
    public interface IGetListEnteredPricesRepository
    {
        Task<List<AuctionRoundPrices>?> GetListEnteredPricesAsync(Guid auctionRoundId);
    }
}
