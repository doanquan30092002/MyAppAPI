using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyApp.Core.Entities;

namespace MyApp.Application.Interfaces.IGetListAuctionRoundRepository
{
    public interface IGetListAuctionRoundRepository
    {
        Task<List<AuctionRound>> GetAuctionRoundsByAuctionIdAsync(Guid auctionId);
    }
}
