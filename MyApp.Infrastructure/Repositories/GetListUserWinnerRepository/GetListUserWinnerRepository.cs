using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MyApp.Application.CQRS.GetListUserWinner.Queries;
using MyApp.Application.CQRS.GetListUserWinner.Querries;
using MyApp.Application.Interfaces.IListUserWinnerRepository;
using MyApp.Core.Entities;
using MyApp.Infrastructure.Data;

namespace MyApp.Infrastructure.Repositories.GetListUserWinnerRepository
{
    public class GetListUserWinnerRepository : IListUserWinnerRepository
    {
        private readonly AppDbContext _context;

        public GetListUserWinnerRepository(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<GetListUserWinnerResponse> GetListUserWinnerAsync(
            GetListUserWinnerRequest getListUserWinnerRequest
        )
        {
            if (
                getListUserWinnerRequest == null
                || getListUserWinnerRequest.AuctionId == Guid.Empty
            )
            {
                throw new ArgumentException(
                    "ID phiên không hợp lệ",
                    nameof(getListUserWinnerRequest)
                );
            }

            // Lấy RoundNumber cao nhất cho AuctionId
            var maxRoundNumber = await _context
                .AuctionRounds.Where(ar => ar.AuctionId == getListUserWinnerRequest.AuctionId)
                .MaxAsync(ar => ar.RoundNumber);

            var query = _context
                .AuctionRoundPrices.Include(arp => arp.AuctionRound)
                .Where(arp =>
                    arp.AuctionRound.AuctionId == getListUserWinnerRequest.AuctionId
                    && arp.AuctionRound.RoundNumber == maxRoundNumber
                    && arp.FlagWinner == true
                );

            var winners = await query.ToListAsync(); // Lấy dữ liệu từ DB
            var groupedWinners = winners
                .GroupBy(arp => arp.TagName)
                .SelectMany(g => g.OrderByDescending(arp => arp.AuctionPrice))
                .ToList();

            return new GetListUserWinnerResponse { auctionRoundPrices = groupedWinners };
        }
    }
}
