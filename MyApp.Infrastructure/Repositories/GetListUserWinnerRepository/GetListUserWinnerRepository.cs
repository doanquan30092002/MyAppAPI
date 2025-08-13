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

            // Check if the auction is completed (Status = 2)
            var auction = await _context.Auctions.FirstOrDefaultAsync(a =>
                a.AuctionId == getListUserWinnerRequest.AuctionId
            );

            if (auction == null || auction.Status != 2)
            {
                return new GetListUserWinnerResponse
                {
                    auctionRoundPrices = new List<AuctionRoundPrices>(),
                };
            }

            // Get all assets for the auction
            var assets = await _context
                .AuctionAssets.Where(aa => aa.AuctionId == getListUserWinnerRequest.AuctionId)
                .ToListAsync();

            if (!assets.Any())
            {
                return new GetListUserWinnerResponse
                {
                    auctionRoundPrices = new List<AuctionRoundPrices>(),
                };
            }

            var winners = new List<AuctionRoundPrices>();

            // For each asset, find the max round and get winners
            foreach (var asset in assets)
            {
                var maxRoundForTag =
                    await _context
                        .AuctionRoundPrices.Where(arp =>
                            arp.TagName == asset.TagName
                            && arp.AuctionRound.AuctionId == getListUserWinnerRequest.AuctionId
                        )
                        .MaxAsync(arp => (int?)arp.AuctionRound.RoundNumber) ?? 0;

                if (maxRoundForTag == 0)
                    continue;

                // Get winners for this asset in its max round
                var tagWinners = await _context
                    .AuctionRoundPrices.Include(arp => arp.AuctionRound)
                    .Where(arp =>
                        arp.AuctionRound.AuctionId == getListUserWinnerRequest.AuctionId
                        && arp.AuctionRound.RoundNumber == maxRoundForTag
                        && arp.TagName == asset.TagName
                        && arp.FlagWinner == true
                    )
                    .OrderByDescending(arp => arp.AuctionPrice)
                    .ToListAsync();

                winners.AddRange(tagWinners);
            }

            return new GetListUserWinnerResponse { auctionRoundPrices = winners };
        }
    }
}
