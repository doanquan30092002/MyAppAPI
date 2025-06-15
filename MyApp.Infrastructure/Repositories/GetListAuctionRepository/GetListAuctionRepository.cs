using Microsoft.EntityFrameworkCore;
using MyApp.Application.CQRS.Auction.GetListAuction.Querries;
using MyApp.Application.CQRS.Auction.GetListAution.Querries;
using MyApp.Application.Interfaces.IGetListRepository;
using MyApp.Infrastructure.Data;

namespace MyApp.Infrastructure.Repositories.GetListAuctionRepository
{
    public class GetListAuctionRepository : IGetListRepository
    {
        private readonly AppDbContext context;

        public GetListAuctionRepository(AppDbContext context)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<GetListAuctionResponse> GetListAuctionsAsync(
            GetListAuctionRequest getListAuctionRequest
        )
        {
            try
            {
                // Initialize query with includes
                var query = context
                    .Auctions.Include(a => a.CreatedByUser)
                    .Include(a => a.Category)
                    .AsQueryable();

                // Filter by AuctionName if provided (case-insensitive)
                if (!string.IsNullOrEmpty(getListAuctionRequest.AuctionName))
                {
                    query = query.Where(a =>
                        a.AuctionName.ToLower()
                            .Contains(getListAuctionRequest.AuctionName.ToLower())
                    );
                }

                // Filter by CategoryId if provided
                if (getListAuctionRequest.CategoryId.HasValue)
                {
                    query = query.Where(a =>
                        a.CategoryId == getListAuctionRequest.CategoryId.Value
                    );
                }

                // Filter by RegisterOpenDate and RegisterEndDate
                if (
                    getListAuctionRequest.RegisterOpenDate.HasValue
                    && getListAuctionRequest.RegisterEndDate.HasValue
                )
                {
                    query = query.Where(a =>
                        a.RegisterOpenDate >= getListAuctionRequest.RegisterOpenDate.Value
                        && a.RegisterEndDate <= getListAuctionRequest.RegisterEndDate.Value
                    );
                }
                else if (getListAuctionRequest.RegisterOpenDate.HasValue)
                {
                    query = query.Where(a =>
                        a.RegisterOpenDate >= getListAuctionRequest.RegisterOpenDate.Value
                    );
                }
                else if (getListAuctionRequest.RegisterEndDate.HasValue)
                {
                    query = query.Where(a =>
                        a.RegisterEndDate <= getListAuctionRequest.RegisterEndDate.Value
                    );
                }

                // Filter by AuctionStartDate and AuctionEndDate
                if (
                    getListAuctionRequest.AuctionStartDate.HasValue
                    && getListAuctionRequest.AuctionEndDate.HasValue
                )
                {
                    query = query.Where(a =>
                        a.AuctionStartDate >= getListAuctionRequest.AuctionStartDate.Value
                        && a.AuctionEndDate <= getListAuctionRequest.AuctionEndDate.Value
                    );
                }
                else if (getListAuctionRequest.AuctionStartDate.HasValue)
                {
                    query = query.Where(a =>
                        a.AuctionStartDate >= getListAuctionRequest.AuctionStartDate.Value
                    );
                }
                else if (getListAuctionRequest.AuctionEndDate.HasValue)
                {
                    query = query.Where(a =>
                        a.AuctionEndDate <= getListAuctionRequest.AuctionEndDate.Value
                    );
                }

                // Calculate total count before pagination
                int totalCount = await query.CountAsync();

                // Sort by specified field
                if (!string.IsNullOrEmpty(getListAuctionRequest.SortBy))
                {
                    switch (getListAuctionRequest.SortBy.ToLower())
                    {
                        case "auction_name":
                            query = getListAuctionRequest.IsAscending
                                ? query.OrderBy(a => a.AuctionName)
                                : query.OrderByDescending(a => a.AuctionName);
                            break;
                        case "register_open_date":
                            query = getListAuctionRequest.IsAscending
                                ? query.OrderBy(a => a.RegisterOpenDate)
                                : query.OrderByDescending(a => a.RegisterOpenDate);
                            break;
                        case "register_end_date":
                            query = getListAuctionRequest.IsAscending
                                ? query.OrderBy(a => a.RegisterEndDate)
                                : query.OrderByDescending(a => a.RegisterEndDate);
                            break;
                        case "auction_start_date":
                            query = getListAuctionRequest.IsAscending
                                ? query.OrderBy(a => a.AuctionStartDate)
                                : query.OrderByDescending(a => a.AuctionStartDate);
                            break;
                        case "auction_end_date":
                            query = getListAuctionRequest.IsAscending
                                ? query.OrderBy(a => a.AuctionEndDate)
                                : query.OrderByDescending(a => a.AuctionEndDate);
                            break;
                        default:
                            query = query.OrderBy(a => a.CreatedAt); // Default sort by CreatedAt
                            break;
                    }
                }
                else
                {
                    query = query.OrderBy(a => a.CreatedAt); // Default sort by CreatedAt
                }

                // Apply pagination
                var pageNumber = getListAuctionRequest.PageNumber ?? 1;
                var pageSize = getListAuctionRequest.PageSize ?? 2;

                query = query.Skip((pageNumber - 1) * pageSize).Take(pageSize);

                // Map to ListAuctionDTO
                var auctions = await query
                    .Select(a => new ListAuctionDTO
                    {
                        AuctionId = a.AuctionId,
                        AuctionName = a.AuctionName,
                        CategoryId = a.CategoryId,
                        RegisterOpenDate = a.RegisterOpenDate,
                        RegisterEndDate = a.RegisterEndDate,
                        AuctionStartDate = a.AuctionStartDate,
                        AuctionEndDate = a.AuctionEndDate,
                        CreatedByUserName = a.CreatedByUser.Name,
                    })
                    .ToListAsync();

                // Create response
                var response = new GetListAuctionResponse
                {
                    TotalCount = totalCount,
                    Auctions = auctions,
                };

                return response;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while retrieving the auction list.", ex);
            }
        }
    }
}
