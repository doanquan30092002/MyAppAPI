using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
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
                    .Where(x => x.Status_deposit == true)
                    .Select(x => x.NumericalOrder)
                    .FirstOrDefaultAsync();
                if (numericalOrder != null)
                {
                    return new SearchUserAttendanceResponse
                    {
                        Message = "Tìm thấy số thứ tự.",
                        AuctionName = auctionSearch.AuctionName,
                        NumericalOrder = numericalOrder,
                    };
                }
                else
                {
                    return new SearchUserAttendanceResponse
                    {
                        Message = "Không tìm thấy số thứ tự.",
                        AuctionName = auctionSearch.AuctionName,
                        NumericalOrder = null,
                    };
                }
            }
            return new SearchUserAttendanceResponse
            {
                Message = "Không tồn tại phiên đấu giá này.",
                AuctionName = null,
                NumericalOrder = null,
            };
        }
    }
}
