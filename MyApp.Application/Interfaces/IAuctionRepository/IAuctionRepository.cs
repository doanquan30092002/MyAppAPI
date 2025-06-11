using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyApp.Application.CQRS.Auction.AddAuction.Commands;

namespace MyApp.Application.Interfaces.IAuctionRepository
{
    public interface IAuctionRepository
    {
        Task<Guid> AddAuctionAsync(AddAuctionCommand command, Guid userId);
    }
}
