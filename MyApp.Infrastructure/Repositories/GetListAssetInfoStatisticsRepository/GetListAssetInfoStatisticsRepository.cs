using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MyApp.Application.CQRS.GetListAssetInfoStatistic.Queries;
using MyApp.Application.Interfaces.IGetListAssetInfostatisticsRepository;
using MyApp.Infrastructure.Data;

namespace MyApp.Infrastructure.Repositories.GetListAssetInfoStatisticsRepository
{
    public class GetListAssetInfoStatisticsRepository : IGetListAssetInfostatisticsRepository
    {
        private readonly AppDbContext context;

        public GetListAssetInfoStatisticsRepository(AppDbContext context)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<GetListAssetInfoStatisticsResponse> GetAuctionAssetsStatistics(
            GetListAssetInfostatisticsRequest request
        )
        {
            // Query AuctionAssets to get AssetName, StartingPrice, and AuctionId
            var auctionAsset = await context
                .AuctionAssets.Where(aa => aa.AuctionAssetsId == request.AuctionAssetsId)
                .Select(aa => new
                {
                    aa.TagName,
                    aa.StartingPrice,
                    aa.AuctionId,
                })
                .FirstOrDefaultAsync();

            if (auctionAsset == null)
            {
                throw new Exception("Auction asset not found.");
            }

            // Query Auction to check if it has completed (Status = 2)
            var auctionStatus = await context
                .Auctions.Where(a => a.AuctionId == auctionAsset.AuctionId)
                .Select(a => a.Status)
                .FirstOrDefaultAsync();

            // Query AuctionRound to get all rounds for the auction
            var auctionRounds = await context
                .AuctionRounds.Where(ar => ar.AuctionId == auctionAsset.AuctionId)
                .Select(ar => ar.AuctionRoundId)
                .ToListAsync();

            // Query AuctionRoundPrices to get TotalBids, HighestPrice, and HasWinner
            var roundPrices = await context
                .AuctionRoundPrices.Where(arp =>
                    auctionRounds.Contains(arp.AuctionRoundId)
                    && arp.TagName == auctionAsset.TagName
                )
                .ToListAsync();

            var totalBids = roundPrices.Count;
            var highestPrice = roundPrices.Any() ? roundPrices.Max(arp => arp.AuctionPrice) : 0m;
            var hasWinner = auctionStatus == 2 && roundPrices.Any(arp => arp.FlagWinner);

            // Query AuctionDocuments to get TotalParticipants (count where IsAttended = true)
            var totalParticipants = await context
                .AuctionDocuments.Where(ad =>
                    ad.AuctionAssetId == request.AuctionAssetsId && ad.IsAttended == true
                )
                .CountAsync();

            // Construct the response
            var response = new GetListAssetInfoStatisticsResponse
            {
                AssetName = auctionAsset.TagName,
                StartingPrice = auctionAsset.StartingPrice,
                TotalBids = totalBids,
                HighestPrice = highestPrice,
                TotalParticipants = totalParticipants,
                HasWinner = hasWinner,
            };

            return response;
        }
    }
}
