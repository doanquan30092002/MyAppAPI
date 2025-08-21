using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MyApp.Application.CQRS.GetStatisticOverview.Queries;
using MyApp.Application.Interfaces.IGetStatisticOverviewRepository;
using MyApp.Infrastructure.Data;

namespace MyApp.Infrastructure.Repositories.GetStatisticOverviewRepository
{
    public class GetStatisticOverviewRepository : IGetStatisticOverviewRepository
    {
        private readonly AppDbContext _context;

        public GetStatisticOverviewRepository(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<GetStatisticOverviewResponse> GetStatisticOverview(
            GetStatisticOverviewRequest request
        )
        {
            if (request == null)
            {
                throw new ArgumentException("Invalid request", nameof(request));
            }

            // Get the current month and year if not provided in the request
            int month = request.Month ?? DateTime.Now.Month;
            int year = request.Year ?? DateTime.Now.Year;
            var startDate = new DateTime(year, month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1); // Last day of the month

            // Validate month and year values
            if (month < 1 || month > 12 || year < 1)
            {
                throw new ArgumentException(
                    "Month must be between 1 and 12, and year must be valid",
                    nameof(request)
                );
            }

            // Retrieve all auctions within the given month and year
            var auctions = await _context
                .Auctions.Where(a =>
                    a.AuctionStartDate.Month == month && a.AuctionStartDate.Year == year
                )
                .ToListAsync();

            // Count the number of successful auctions (Status = 2)
            var successfulAuctions = auctions.Count(a => a.Status == 2);

            // Retrieve all auction asset IDs related to these auctions
            var auctionAssetIds = auctions
                .SelectMany(a =>
                    _context
                        .AuctionAssets.Where(aa => aa.AuctionId == a.AuctionId)
                        .Select(aa => aa.AuctionAssetsId)
                )
                .Distinct()
                .ToList();

            // Retrieve all auction documents related to the auction assets
            var auctionDocuments = await _context
                .AuctionDocuments.Include(ad => ad.AuctionAsset)
                .Where(ad => auctionAssetIds.Contains(ad.AuctionAssetId))
                .ToListAsync();

            // Calculate total revenue (sum of RegistrationFee where StatusTicket = 1)
            var totalRevenue = auctionDocuments
                .Where(ad => ad.StatusTicket == 1 || ad.StatusTicket == 2)
                .Sum(ad => ad.AuctionAsset.RegistrationFee);

            // Calculate total participants (distinct UserId where IsAttended = true)
            var totalParticipants = auctionDocuments
                .Where(ad => ad.IsAttended == true)
                .Select(ad => ad.UserId)
                .Distinct()
                .Count();

            return new GetStatisticOverviewResponse
            {
                TotalRevenue = totalRevenue,
                SuccessfulAuctions = successfulAuctions,
                TotalParticipants = totalParticipants,
            };
        }
    }
}
