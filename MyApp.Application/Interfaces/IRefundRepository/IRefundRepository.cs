using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyApp.Application.CQRS.AuctionDocuments.ConfirmReufund;
using MyApp.Core.Entities;

namespace MyApp.Application.Interfaces.IRefundRepository
{
    public interface IRefundRepository
    {
        Task<bool> ConfirmRefundAsync(ConfirmRefundCommand request);

        /// <summary>
        /// Lấy danh sách AuctionDocuments theo danh sách GUID.
        /// </summary>
        /// <param name="auctionDocumentIds">Danh sách GUID của hồ sơ đấu giá.</param>
        /// <returns>Danh sách AuctionDocuments tương ứng.</returns>
        Task<List<AuctionDocuments>> GetAuctionDocumentsByIdsAsync(List<Guid> auctionDocumentIds);
    }
}
