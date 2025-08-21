using MediatR;
using MyApp.Application.Interfaces.ListAuctionRegisted;

namespace MyApp.Application.CQRS.ListAuctionRegisted
{
    public class AuctionRegistedHandler
        : IRequestHandler<AuctionRegistedRequest, AuctionRegistedResponse>
    {
        private readonly IAuctionRegistedRepository _repository;

        public AuctionRegistedHandler(IAuctionRegistedRepository repository)
        {
            _repository = repository;
        }

        public async Task<AuctionRegistedResponse> Handle(
            AuctionRegistedRequest request,
            CancellationToken cancellationToken
        )
        {
            var assetIds = await _repository.GetRegisteredAssetIdsAsync(request.UserId);
            if (!assetIds.Any())
            {
                return new AuctionRegistedResponse
                {
                    PageNumber = request.PageNumber,
                    PageSize = request.PageSize,
                    TotalAuctionRegisted = 0,
                    AuctionResponse = null,
                };
            }

            var auctionIds = await _repository.GetRegisteredAuctionIdsAsync(assetIds);
            if (!auctionIds.Any())
            {
                return new AuctionRegistedResponse
                {
                    PageNumber = request.PageNumber,
                    PageSize = request.PageSize,
                    TotalAuctionRegisted = 0,
                    AuctionResponse = null,
                };
            }

            var auctions = await _repository.GetAuctionsByIdsAsync(auctionIds);

            // Apply search filters
            if (!string.IsNullOrWhiteSpace(request.Search?.AuctionName))
            {
                auctions = auctions
                    .Where(a =>
                        a.AuctionName.Contains(
                            request.Search.AuctionName,
                            StringComparison.OrdinalIgnoreCase
                        )
                    )
                    .ToList();
            }
            if (request.Search?.AuctionStartDate.HasValue == true)
            {
                auctions = auctions
                    .Where(a => a.AuctionStartDate >= request.Search.AuctionStartDate.Value)
                    .ToList();
            }
            if (request.Search?.AuctionEndDate.HasValue == true)
            {
                auctions = auctions
                    .Where(a => a.AuctionEndDate <= request.Search.AuctionEndDate.Value)
                    .ToList();
            }

            // Map data
            var auctionResponses = new List<AuctionAndAuctionAssetResponse>();
            foreach (var auction in auctions)
            {
                var assets = await _repository.GetAuctionAssetsByAuctionIdAsync(
                    auction.AuctionId,
                    assetIds
                );

                auctionResponses.Add(
                    new AuctionAndAuctionAssetResponse
                    {
                        AuctionId = auction.AuctionId,
                        AuctionName = auction.AuctionName,
                        CategoryName = auction.CategoryName,
                        AuctionDescription = auction.AuctionDescription,
                        AuctionRules = auction.AuctionRules,
                        AuctionPlanningMap = auction.AuctionPlanningMap,
                        RegisterOpenDate = auction.RegisterOpenDate,
                        RegisterEndDate = auction.RegisterEndDate,
                        AuctionStartDate = auction.AuctionStartDate,
                        AuctionEndDate = auction.AuctionEndDate,
                        NumberRoundMax = auction.NumberRoundMax,
                        Status = auction.Status,
                        AuctionAssets = assets
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
                    }
                );
            }

            // Pagination
            var skip = (request.PageNumber - 1) * request.PageSize;
            var pagedResult = auctionResponses.Skip(skip).Take(request.PageSize).ToList();

            return new AuctionRegistedResponse
            {
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalAuctionRegisted = auctionResponses.Count,
                AuctionResponse = pagedResult,
            };
        }
    }
}
