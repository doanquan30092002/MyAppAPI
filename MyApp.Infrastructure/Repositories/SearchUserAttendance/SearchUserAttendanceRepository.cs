using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MyApp.Application.Common.Message;
using MyApp.Application.CQRS.SearchUserAttendance.Queries;
using MyApp.Application.Interfaces.SearchUserAttendance;
using MyApp.Core.Entities;
using MyApp.Infrastructure.Data;

namespace MyApp.Infrastructure.Repositories.SearchUserAttendance
{
    public class SearchUserAttendanceRepository : ISearchUserAttendanceRepository
    {
        private readonly AppDbContext context;

        public SearchUserAttendanceRepository(AppDbContext context)
        {
            this.context = context;
        }

        public async Task<SearchUserAttendanceResponse> SearchUserAttendanceAsync(
            Guid auctionId,
            string citizenIdentification
        )
        {
            var auctionSearch = await context.Auctions.FindAsync(auctionId);
            if (auctionSearch != null)
            {
                var numericalOrder = await context
                    .AuctionDocuments.Include(x => x.AuctionAsset)
                    .ThenInclude(x => x.Auction)
                    .Where(x => x.AuctionAsset.Auction.AuctionId.Equals(auctionId))
                    .Include(x => x.User)
                    .Where(x => x.User.CitizenIdentification.Equals(citizenIdentification))
                    .Where(x => x.StatusTicket == true)
                    .Where(x => x.StatusDeposit == true)
                    .Select(x => x.NumericalOrder)
                    .FirstOrDefaultAsync();
                if (numericalOrder != null)
                {
                    return new SearchUserAttendanceResponse
                    {
                        Message = Message.FOUND_NUMERICAL_ORDER,
                        AuctionName = auctionSearch.AuctionName,
                        NumericalOrder = numericalOrder,
                    };
                }
                else
                {
                    return new SearchUserAttendanceResponse
                    {
                        Message = Message.NOT_FOUND_NUMERICAL_ORDER,
                        AuctionName = auctionSearch.AuctionName,
                        NumericalOrder = null,
                    };
                }
            }
            return new SearchUserAttendanceResponse
            {
                Message = Message.AUCTION_NOT_EXIST,
                AuctionName = null,
                NumericalOrder = null,
            };
        }
    }
}
