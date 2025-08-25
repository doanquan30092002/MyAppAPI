using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MyApp.Application.CQRS.CreateAutionRound.Command;
using MyApp.Application.Interfaces.ICreateAuctionRoundRepository;
using MyApp.Core.Entities;
using MyApp.Infrastructure.Data;

namespace MyApp.Infrastructure.Repositories.CreateAuctionRoundRepository
{
    public class CreateAuctionRoundRepository : ICreateAuctionRoundRepository
    {
        private readonly AppDbContext context;

        public CreateAuctionRoundRepository(AppDbContext context)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<bool> InsertAuctionRound(
            CreateAuctionRoundRequest createAuctionRoundRequest
        )
        {
            var previousRound = await context
                .AuctionRounds.Where(ar => ar.AuctionId == createAuctionRoundRequest.AuctionId)
                .OrderByDescending(ar => ar.RoundNumber)
                .FirstOrDefaultAsync();

            int roundNumber = previousRound != null ? previousRound.RoundNumber + 1 : 1;

            var auctionRound = new AuctionRound
            {
                AuctionRoundId = new Guid(),
                AuctionId = createAuctionRoundRequest.AuctionId,
                RoundNumber = roundNumber,
                Status = 1,
                CreatedAt = DateTime.Now,
                CreatedBy = createAuctionRoundRequest.CreatedBy,
                PriceMin = createAuctionRoundRequest.PriceMin,
                PriceMax = createAuctionRoundRequest.PriceMax,
                TotalPriceMax = createAuctionRoundRequest.TotalPriceMax,
            };

            await context.AuctionRounds.AddAsync(auctionRound);

            var result = await context.SaveChangesAsync();
            return result > 0;
        }
    }
}
