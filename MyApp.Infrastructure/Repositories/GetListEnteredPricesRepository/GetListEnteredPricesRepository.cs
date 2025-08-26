using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MyApp.Application.CQRS.GetListEnteredPrices.Querries;
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

        public async Task<GetListEnteredPricesResponse?> GetListEnteredPricesAsync(
            Guid auctionRoundId
        )
        {
            // Lấy thông tin AuctionRound và Auction
            var auctionRound = await _context
                .AuctionRounds.Include(ar => ar.Auction)
                .FirstOrDefaultAsync(ar => ar.AuctionRoundId == auctionRoundId);

            if (auctionRound == null)
                return null;

            // Lấy danh sách AuctionRoundPrices của round hiện tại
            var auctionRoundPrices = await _context
                .AuctionRoundPrices.Where(arp => arp.AuctionRoundId == auctionRoundId)
                .ToListAsync();

            if (!auctionRoundPrices.Any())
                return new GetListEnteredPricesResponse { Items = new List<EnteredPriceDto>() };

            // Lấy danh sách AuctionAssets của auction
            var auctionAssets = await _context
                .AuctionAssets.Where(aa => aa.AuctionId == auctionRound.AuctionId)
                .ToListAsync();

            // Dictionary để lưu starting price theo TagName
            var startingPrices = new Dictionary<string, decimal>();

            if (auctionRound.RoundNumber == 1)
            {
                // Vòng 1: lấy StartingPrice từ AuctionAssets
                foreach (var asset in auctionAssets)
                {
                    startingPrices[asset.TagName] = asset.StartingPrice;
                }
            }
            else
            {
                // Từ vòng 2 trở đi: lấy giá cao nhất của từng asset ở vòng trước
                var previousRound = await _context.AuctionRounds.FirstOrDefaultAsync(ar =>
                    ar.AuctionId == auctionRound.AuctionId
                    && ar.RoundNumber == auctionRound.RoundNumber - 1
                );

                if (previousRound != null)
                {
                    // Lấy giá cao nhất của từng TagName trong vòng trước
                    var previousRoundMaxPrices = await _context
                        .AuctionRoundPrices.Where(arp =>
                            arp.AuctionRoundId == previousRound.AuctionRoundId
                        )
                        .GroupBy(arp => arp.TagName)
                        .Select(g => new { TagName = g.Key, MaxPrice = g.Max(x => x.AuctionPrice) })
                        .ToListAsync();

                    foreach (var maxPrice in previousRoundMaxPrices)
                    {
                        startingPrices[maxPrice.TagName] = maxPrice.MaxPrice;
                    }

                    // Nếu có asset chưa có giá ở vòng trước, lấy StartingPrice từ AuctionAssets
                    foreach (var asset in auctionAssets)
                    {
                        if (!startingPrices.ContainsKey(asset.TagName))
                        {
                            startingPrices[asset.TagName] = asset.StartingPrice;
                        }
                    }
                }
                else
                {
                    // Fallback: nếu không tìm thấy vòng trước, dùng StartingPrice từ AuctionAssets
                    foreach (var asset in auctionAssets)
                    {
                        startingPrices[asset.TagName] = asset.StartingPrice;
                    }
                }
            }

            // Map sang EnteredPriceDto
            var enteredPriceDtos = auctionRoundPrices
                .Select(arp => new EnteredPriceDto
                {
                    AuctionRoundPriceId = arp.AuctionRoundPriceId,
                    AuctionRoundId = arp.AuctionRoundId,
                    UserName = arp.UserName,
                    CitizenIdentification = arp.CitizenIdentification,
                    RecentLocation = arp.RecentLocation,
                    TagName = arp.TagName,
                    AuctionPrice = arp.AuctionPrice,
                    CreatedAt = arp.CreatedAt,
                    CreatedBy = arp.CreatedBy,
                    FlagWinner = arp.FlagWinner,
                    StartingPrice = startingPrices.ContainsKey(arp.TagName)
                        ? startingPrices[arp.TagName]
                        : (decimal?)null,
                })
                .ToList();

            return new GetListEnteredPricesResponse { Items = enteredPriceDtos };
        }
    }
}
