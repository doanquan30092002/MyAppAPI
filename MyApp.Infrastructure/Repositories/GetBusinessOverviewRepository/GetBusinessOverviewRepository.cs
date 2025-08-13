using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MyApp.Application.CQRS.GetBusinessOverview.Queries;
using MyApp.Application.Interfaces.IGetBusinessOverviewRepository;
using MyApp.Infrastructure.Data;

namespace MyApp.Infrastructure.Repositories.GetBusinessOverviewRepository
{
    public class GetBusinessOverviewRepository : IGetBusinessOverviewRepository
    {
        private readonly AppDbContext context;

        public GetBusinessOverviewRepository(AppDbContext context)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<GetBusinessOverviewResponse> GetBusinessOverview(
            GetBusinessOverviewRequest request
        )
        {
            // Build the base query for auctions
            var auctionQuery = context.Auctions.AsQueryable();

            // Apply CategoryId filter if provided
            if (request.CategoryId.HasValue)
            {
                auctionQuery = auctionQuery.Where(a => a.CategoryId == request.CategoryId.Value);
            }

            // Apply date range filters only if valid dates are provided
            if (request.AuctionStartDate != default(DateTime))
            {
                auctionQuery = auctionQuery.Where(a =>
                    a.AuctionStartDate >= request.AuctionStartDate
                );
            }

            if (request.AuctionEndDate != default(DateTime))
            {
                auctionQuery = auctionQuery.Where(a => a.AuctionEndDate <= request.AuctionEndDate);
            }

            // Calculate TotalAuctions
            var totalAuctions = await auctionQuery.CountAsync();

            // Calculate TotalSuccessfulAuctions (Status = 2)
            var totalSuccessfulAuctions = await auctionQuery.Where(a => a.Status == 2).CountAsync();

            // Calculate TotalCancelledAuctions (Status = 3)
            var totalCancelledAuctions = await auctionQuery.Where(a => a.Status == 3).CountAsync();

            // Calculate TotalParticipants
            var totalParticipants = await context
                .AuctionDocuments.Where(ad =>
                    context
                        .AuctionAssets.Where(aa =>
                            auctionQuery.Select(a => a.AuctionId).Contains(aa.AuctionId)
                        )
                        .Select(aa => aa.AuctionAssetsId)
                        .Contains(ad.AuctionAssetId)
                )
                .Select(ad => ad.UserId)
                .Distinct()
                .CountAsync();

            // Construct response
            var response = new GetBusinessOverviewResponse
            {
                TotalAuctions = totalAuctions,
                TotalParticipants = totalParticipants,
                TotalSuccessfulAuctions = totalSuccessfulAuctions,
                TotalCancelledAuctions = totalCancelledAuctions,
            };

            return response;
        }
    }
}
