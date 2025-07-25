using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MyApp.Application.CQRS.AuctionDocuments.FindHighestPriceAndFlag.Queries;
using MyApp.Application.Interfaces.IAuctionRepository;
using MyApp.Application.Interfaces.IFindHighestPriceAndFlag;
using MyApp.Infrastructure.Data;
using MyApp.Infrastructure.Repositories.AuctionRepository;

namespace MyApp.Infrastructure.Repositories.FindHighestPriceAndFlagRepository
{
    public class FindHighestPriceAndFlagRepository : IFindHighestPriceAndFlag
    {
        private readonly AppDbContext _context;
        private readonly IAuctionRepository _auctionRepository;

        public FindHighestPriceAndFlagRepository(
            AppDbContext context,
            IAuctionRepository auctionRepository
        )
        {
            _context = context;
            _auctionRepository = auctionRepository;
        }

        public async Task<FindHighestPriceAndFlagResponse> FindHighestPriceAndFlag(
            Guid auctionId,
            Guid userId
        )
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                throw new KeyNotFoundException("Không tìm thấy người dùng.");

            var auction = await _auctionRepository.FindAuctionByIdAsync(auctionId);

            if (auction == null)
            {
                throw new KeyNotFoundException("Không tìm thấy phiên đấu giá: " + auctionId);
            }

            var citizenId = user.CitizenIdentification;

            var auctionDocuments = await _context
                .AuctionDocuments.Include(ad => ad.AuctionAsset)
                .Where(ad => ad.UserId == userId && ad.AuctionAsset.AuctionId == auctionId)
                .ToListAsync();

            var result = new Dictionary<Guid, List<PriceFlagDto>>();

            foreach (var doc in auctionDocuments)
            {
                var tagName = doc.AuctionAsset.TagName;

                var latestRound = await (
                    from price in _context.AuctionRoundPrices
                    join round in _context.AuctionRounds
                        on price.AuctionRoundId equals round.AuctionRoundId
                    where
                        round.AuctionId == auctionId
                        && price.TagName == tagName
                        && price.CitizenIdentification == citizenId
                    orderby round.RoundNumber descending
                    select new { round.RoundNumber, round.AuctionRoundId }
                ).FirstOrDefaultAsync();

                if (latestRound == null)
                    continue;

                var highestPrice = await _context
                    .AuctionRoundPrices.Where(p =>
                        p.AuctionRoundId == latestRound.AuctionRoundId
                        && p.TagName == tagName
                        && p.CitizenIdentification == citizenId
                    )
                    .OrderByDescending(p => p.AuctionPrice)
                    .Select(p => new PriceFlagDto { Price = p.AuctionPrice, Flag = p.FlagWinner })
                    .FirstOrDefaultAsync();

                if (highestPrice != null)
                {
                    result[doc.AuctionDocumentsId] = new List<PriceFlagDto> { highestPrice };
                }
            }

            return new FindHighestPriceAndFlagResponse { Data = result };
        }
    }
}
