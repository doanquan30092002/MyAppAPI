using MediatR;
using MyApp.Application.Interfaces.ListAuctionAsset;

namespace MyApp.Application.CQRS.ListAuctionAsset
{
    public class ListAuctionAssetHandler
        : IRequestHandler<ListAuctionAssetRequest, ListAuctionAssetResponse>
    {
        private readonly IListAuctionAssetRepository _repository;

        public ListAuctionAssetHandler(IListAuctionAssetRepository repository)
        {
            _repository = repository;
        }

        public async Task<ListAuctionAssetResponse> Handle(
            ListAuctionAssetRequest request,
            CancellationToken cancellationToken
        )
        {
            //get all auction assets
            List<AuctionAssetResponse> auctionAssets = await _repository.GetAllAuctionAssets(
                request.PageNumber,
                request.PageSize,
                request.Search
            );
            //get total auction assets count
            int totalAuctionAsset = await _repository.GetTotalAuctionAssetsCount();
            //get category counts
            Dictionary<string, int> categoryCounts = await _repository.GetCategoryCounts();
            //map to response
            var response = new ListAuctionAssetResponse();

            response = new ListAuctionAssetResponse
            {
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalAuctionAsset = totalAuctionAsset,
                AuctionAssetResponses =
                    auctionAssets.Count > 0 ? auctionAssets : new List<AuctionAssetResponse>(),
                CategoryCounts = categoryCounts,
            };
            return response;
        }
    }
}
