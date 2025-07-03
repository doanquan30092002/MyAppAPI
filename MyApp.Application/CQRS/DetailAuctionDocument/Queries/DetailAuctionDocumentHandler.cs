using MediatR;
using MyApp.Application.Interfaces.DetailAuctionDocument;
using MyApp.Application.Interfaces.UpdateExpiredProfile;

namespace MyApp.Application.CQRS.DetailAuctionDocument.Queries
{
    public class DetailAuctionDocumentHandler
        : IRequestHandler<DetailAuctionDocumentRequest, DetailAuctionDocumentResponse?>
    {
        private readonly IDetailAuctionDocumentRepository _detailAuctionDocumentRepository;

        public DetailAuctionDocumentHandler(
            IDetailAuctionDocumentRepository detailAuctionDocumentRepository
        )
        {
            _detailAuctionDocumentRepository = detailAuctionDocumentRepository;
        }

        public async Task<DetailAuctionDocumentResponse?> Handle(
            DetailAuctionDocumentRequest request,
            CancellationToken cancellationToken
        )
        {
            var response =
                await _detailAuctionDocumentRepository.GetDetailAuctionDocumentByAuctionDocumentsIdAsync(
                    request.AuctionDocumentsId
                );
            if (response == null)
            {
                return null;
            }
            return response;
        }
    }
}
