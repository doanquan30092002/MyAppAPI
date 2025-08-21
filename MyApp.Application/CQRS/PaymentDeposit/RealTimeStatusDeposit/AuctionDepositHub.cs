using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace MyApp.Application.CQRS.PaymentDeposit.RealTimeStatusDeposit
{
    public class AuctionDepositHub : Hub
    {
        public Task JoinAuctionDocumentGroup(string auctionDocumentId) =>
            Groups.AddToGroupAsync(Context.ConnectionId, auctionDocumentId);

        public Task LeaveAuctionDocumentGroup(string auctionDocumentId) =>
            Groups.RemoveFromGroupAsync(Context.ConnectionId, auctionDocumentId);
    }
}
