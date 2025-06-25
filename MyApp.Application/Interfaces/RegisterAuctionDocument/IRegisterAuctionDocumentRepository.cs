using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyApp.Application.CQRS.RegisterAuctionDocument.Command;

namespace MyApp.Application.Interfaces.RegisterAuctionDocument
{
    public interface IRegisterAuctionDocumentRepository
    {
        Task<bool> CheckAuctionDocumentExsit(string? userId, string auctionAssetsId);
        Task<RegisterAuctionDocumentResponse> CreateQRForPayTicket(Guid auctionDocumentsId);
        Task<Guid> InsertAuctionDocumentAsync(
            string auctionAssetsId,
            string? userId,
            string? bankAccount,
            string? bankAccountNumber,
            string? bankBranch
        );
        Task<bool> UpdateStatusTicketAsync(Guid auctionDocumentsId);
    }
}
