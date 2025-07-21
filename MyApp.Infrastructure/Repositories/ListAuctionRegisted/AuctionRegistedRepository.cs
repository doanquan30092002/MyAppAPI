using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.EntityFrameworkCore;
using MyApp.Application.CQRS.ListAuctionRegisted;
using MyApp.Application.Interfaces.ListAuctionRegisted;
using MyApp.Infrastructure.Data;

namespace MyApp.Infrastructure.Repositories.ListAuctionRegisted
{
    public class AuctionRegistedRepository : IAuctionRegistedRepository
    {
        private readonly AppDbContext _context;

        public AuctionRegistedRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<AuctionRegistedResponse> ListAuctionRegisted(
            string? userId,
            AuctionRegistedRequest request
        )
        {
            if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var userGuid))
                return null;

            // Get all AuctionAssetIds the user has registered in AuctionDocuments
            var registeredAssetIds = await _context
                .AuctionDocuments.Where(ad =>
                    ad.UserId == userGuid && ad.StatusTicket == 2 && ad.StatusDeposit == 1
                )
                .Select(ad => ad.AuctionAssetId)
                .ToListAsync();

            if (!registeredAssetIds.Any())
                return null;

            // Get all AuctionIds from AuctionAssets the user has registered
            var registeredAuctionIds = await _context
                .AuctionAssets.Where(aa => registeredAssetIds.Contains(aa.AuctionAssetsId))
                .Select(aa => aa.AuctionId)
                .Distinct()
                .ToListAsync();

            if (!registeredAuctionIds.Any())
                return null;

            // Get info of all auctions the user has registered
            var auctions = _context.Auctions.AsQueryable();
            if (string.IsNullOrEmpty(request.Search.AuctionName) == false)
            {
                auctions = auctions.Where(a =>
                    a.AuctionName.ToLower().Contains(request.Search.AuctionName.ToLower())
                );
            }
            if (request.Search.AuctionStartDate.HasValue)
            {
                auctions = auctions.Where(a =>
                    a.AuctionStartDate >= request.Search.AuctionStartDate.Value
                );
            }
            if (request.Search.AuctionEndDate.HasValue)
            {
                auctions = auctions.Where(a =>
                    a.AuctionEndDate <= request.Search.AuctionEndDate.Value
                );
            }

            var auctionResponses = auctions
                .Where(a => registeredAuctionIds.Contains(a.AuctionId))
                .Select(a => new AuctionResponse
                {
                    AuctionId = a.AuctionId,
                    AuctionName = a.AuctionName,
                    CategoryName = a.Category.CategoryName,
                    AuctionDescription = a.AuctionDescription,
                    AuctionRules = a.AuctionRules,
                    AuctionPlanningMap = a.AuctionPlanningMap,
                    RegisterOpenDate = a.RegisterOpenDate,
                    RegisterEndDate = a.RegisterEndDate,
                    AuctionStartDate = a.AuctionStartDate,
                    AuctionEndDate = a.AuctionEndDate,
                    NumberRoundMax = a.NumberRoundMax,
                    Status = a.Status,
                    AuctionAssets = _context
                        .AuctionAssets.Where(aa =>
                            aa.AuctionId == a.AuctionId
                            && registeredAssetIds.Contains(aa.AuctionAssetsId)
                        )
                        .Select(aa => new AuctionAsset
                        {
                            AuctionAssetsId = aa.AuctionAssetsId,
                            TagName = aa.TagName,
                            StartingPrice = aa.StartingPrice,
                            Unit = aa.Unit,
                            Deposit = aa.Deposit,
                            RegistrationFee = aa.RegistrationFee,
                            Description = aa.Description,
                            AuctionId = aa.AuctionId,
                        })
                        .ToList(),
                })
                .ToList();
            var skipResult = (request.PageNumber - 1) * request.PageSize;
            var result = auctionResponses.Skip(skipResult).Take(request.PageSize).ToList();

            return new AuctionRegistedResponse
            {
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalAuctionRegisted = result.Count,
                AuctionResponse = result,
            };
        }
    }
}
