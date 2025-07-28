using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MyApp.Application.Interfaces.IUpdateWinnerFlagRepository;
using MyApp.Core.Entities;
using MyApp.Infrastructure.Data;

namespace MyApp.Infrastructure.Repositories.UpdateWinnerFlagRepository
{
    public class UpdateWinnerFlagRepository : IUpdateWinnerFlagRepository
    {
        private readonly AppDbContext _context;

        public UpdateWinnerFlagRepository(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<bool> UpdateWinnerFlagAsync(Guid auctionRoundPriceId)
        {
            var auctionRoundPrice = await _context.AuctionRoundPrices.FirstOrDefaultAsync(arp =>
                arp.AuctionRoundPriceId == auctionRoundPriceId
            );

            if (auctionRoundPrice == null)
            {
                return false;
            }

            auctionRoundPrice.FlagWinner = true;
            int rowsAffected = await _context.SaveChangesAsync();

            return rowsAffected > 0;
        }
    }
}
