using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyApp.Application.CQRS.Auction.GetListAuction.Querries;
using MyApp.Application.CQRS.Auction.GetListAution.Querries;
using MyApp.Application.CQRS.UpdateStatusDeposit.Command;

namespace MyApp.Application.Interfaces.IUpdateDepositStatus
{
    public interface IUpdateDepositStatus
    {
        Task<List<string>?> GetEmailList(Guid auctionDocumentsId);
        Task<int> GetOrderNumber(Guid auctionDocumentsId);
        public Task<UpdateDepositStatusResponse> UpdateDepositStatus(
            UpdateDepositStatusRequest updateDepositStatusRequest,
            CancellationToken cancellationToken
        );
    }
}
