using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MyApp.Application.Interfaces.IGetListEnteredPricesRepository;
using MyApp.Core.Entities;
using MyApp.Infrastructure.Data;

namespace MyApp.Infrastructure.Repositories.GetListEnteredPricesRepository
{
    public class GetListEnteredPricesRepository : IGetListEnteredPricesRepository
    {
        private readonly AppDbContext _context;

        public GetListEnteredPricesRepository(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<List<AuctionRoundPrices>?> GetListEnteredPricesAsync(Guid auctionRoundId)
        {
            return await _context
                .AuctionRoundPrices.Where(arp => arp.AuctionRoundId == auctionRoundId)
                .ToListAsync();
        }
    }
}
