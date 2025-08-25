using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MyApp.Application.CQRS.GetListBidders.Queries;
using MyApp.Application.Interfaces.IGetListBidders;
using MyApp.Core.Entities;
using MyApp.Infrastructure.Data;

namespace MyApp.Infrastructure.Repositories.GetListBiddersRepository
{
    public class GetListBiddersRepository : IGetListBiddersRepository
    {
        private readonly AppDbContext context;

        public GetListBiddersRepository(AppDbContext context)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<GetListBiddersResponse> GetListBidders(GetListBiddersRequest request)
        {
            var bidders = await context
                .AuctionRounds.Where(ar => ar.AuctionId == request.AuctionId)
                .Join(
                    context.AuctionRoundPrices,
                    ar => ar.AuctionRoundId,
                    arp => arp.AuctionRoundId,
                    (ar, arp) => new { arp.CitizenIdentification }
                )
                .Join(
                    context.Users,
                    arp => arp.CitizenIdentification,
                    user => user.CitizenIdentification,
                    (arp, user) => new BidderDto { UserId = user.Id, IsBidPlaced = true }
                )
                .Distinct()
                .ToListAsync();

            // Tạo response
            var response = new GetListBiddersResponse { ListBidders = bidders };

            return response;
        }
    }
}
