using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MyApp.Application.Interfaces.IGetListAuctionRoundRepository;
using MyApp.Core.Entities;
using MyApp.Infrastructure.Data;

namespace MyApp.Infrastructure.Repositories.GetListAuctionRoundRepository
{
    public class GetListAuctionRoundRepository : IGetListAuctionRoundRepository
    {
        private readonly AppDbContext context;

        public GetListAuctionRoundRepository(AppDbContext context)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<List<AuctionRound>> GetAuctionRoundsByAuctionIdAsync(Guid auctionId)
        {
            return await context
                .AuctionRounds.Where(ar => ar.AuctionId == auctionId)
                .OrderBy(ar => ar.RoundNumber)
                .ToListAsync();
        }
    }
}
