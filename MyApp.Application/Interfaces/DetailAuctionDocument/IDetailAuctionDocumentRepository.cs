using MyApp.Application.CQRS.DetailAuctionDocument.Queries;

namespace MyApp.Application.Interfaces.DetailAuctionDocument
{
    public interface IDetailAuctionDocumentRepository
    {
        Task<DetailAuctionDocumentResponse?> GetDetailAuctionDocumentByAuctionDocumentsIdAsync(
            Guid auctionDocumentsId
        );
    }
}
