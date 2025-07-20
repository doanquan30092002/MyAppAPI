using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyApp.Application.CQRS.CreateAutionRound.Command;

namespace MyApp.Application.Interfaces.ICreateAuctionRoundRepository
{
    public interface ICreateAuctionRoundRepository
    {
        Task<bool> InsertAuctionRound(CreateAuctionRoundRequest createAuctionRoundRequest);
    }
}
