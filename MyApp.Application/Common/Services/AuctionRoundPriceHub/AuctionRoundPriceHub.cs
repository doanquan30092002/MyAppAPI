using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace MyApp.Application.Common.Services.AuctionRoundPriceHub
{
    public class AuctionRoundPriceHub : Hub
    {
        public async Task JoinGroup(string auctionRoundId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, auctionRoundId);
        }
    }
}
