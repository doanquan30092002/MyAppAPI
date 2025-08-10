using MediatR;
using MyApp.Application.Interfaces.DetailAuctionAsset;

namespace MyApp.Application.CQRS.DetailAuctionAsset
{
    public class DetailAuctionAssetHandler
        : IRequestHandler<DetailAuctionAssetRequest, DetailAuctionAssetResponse>
    {
        private readonly IDetailAuctionAssetRepository _repository;

        public DetailAuctionAssetHandler(IDetailAuctionAssetRepository repository)
        {
            _repository = repository;
        }

        public async Task<DetailAuctionAssetResponse> Handle(
            DetailAuctionAssetRequest request,
            CancellationToken cancellationToken
        )
        {
            AuctionAssetResponse? auctionAsset = await _repository.GetAuctionAssetByIdAsync(
                request.AuctionAssetsId
            );
            if (auctionAsset == null)
            {
                return new DetailAuctionAssetResponse
                {
                    AuctionAssetResponse = null,
                    TotalAuctionDocument = 0,
                    TotalRegistrationFee = 0,
                    TotalDeposit = 0,
                };
            }
            int totalAuctionDocuments = await _repository.GetTotalAuctionDocumentsAsync(
                request.AuctionAssetsId
            );
            decimal totalRegistrationFee = await _repository.GetTotalRegistrationFeeAsync(
                request.AuctionAssetsId
            );
            decimal totalDeposit = await _repository.GetTotalDepositAsync(request.AuctionAssetsId);
            return new DetailAuctionAssetResponse
            {
                AuctionAssetResponse = auctionAsset,
                TotalAuctionDocument = totalAuctionDocuments,
                TotalRegistrationFee = totalRegistrationFee,
                TotalDeposit = totalDeposit,
            };
        }
    }
}
