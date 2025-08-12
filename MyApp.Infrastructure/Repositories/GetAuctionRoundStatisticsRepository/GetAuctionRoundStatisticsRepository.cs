using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MyApp.Application.CQRS.GetAuctionRoundStatistics.Queries;
using MyApp.Application.Interfaces.IGetAuctionRoundStatisticsRepository;
using MyApp.Core.Entities;
using MyApp.Infrastructure.Data;

namespace MyApp.Infrastructure.Repositories.GetAuctionRoundStatisticsRepository
{
    public class GetAuctionRoundStatisticsRepository : IGetAuctionRoundStatisticsRepository
    {
        private readonly AppDbContext context;

        public GetAuctionRoundStatisticsRepository(AppDbContext context)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<GetAuctionRoundStatisticsResponse> GetAuctionRoundStatistics(
            GetAuctionRoundStatisticsRequest request
        )
        {
            // Count total participants (AuctionDocuments with IsAttended = true)
            var totalParticipants = await context
                .AuctionDocuments.Where(ad =>
                    ad.AuctionAsset.AuctionId == request.AuctionId && ad.IsAttended == true
                )
                .Select(ad => ad.UserId)
                .Distinct()
                .CountAsync();

            // Count total assets (AuctionAssets for the given AuctionId)
            var totalAssets = await context.AuctionAssets.CountAsync(aa =>
                aa.AuctionId == request.AuctionId
            );

            // Count total bids (AuctionRoundPrices for the given AuctionId)
            var totalBids = await context
                .AuctionRoundPrices.Where(arp => arp.AuctionRound.AuctionId == request.AuctionId)
                .CountAsync();

            return new GetAuctionRoundStatisticsResponse
            {
                TotalParticipants = totalParticipants,
                TotalAssets = totalAssets,
                TotalBids = totalBids,
            };
        }
    }
}
