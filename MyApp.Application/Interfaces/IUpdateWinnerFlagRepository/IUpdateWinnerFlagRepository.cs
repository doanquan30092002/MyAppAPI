using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyApp.Application.Interfaces.IUpdateWinnerFlagRepository
{
    public interface IUpdateWinnerFlagRepository
    {
        Task<bool> UpdateWinnerFlagAsync(Guid auctionRoundPriceId);
    }
}
