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
            GetListAuctionRequest getListAuctionRequest,
            string? userId = null
        )
        {
            try
            {
                // Initialize query with joins
                var query =
                    from auction in context.Auctions
                    join category in context.AuctionCategories
                        on auction.CategoryId equals category.CategoryId
                    join createdUser in context.Users on auction.CreatedBy equals createdUser.Id
                    join updatedUser in context.Users on auction.UpdatedBy equals updatedUser.Id
                    join auctioneerUser in context.Users
                        on auction.Auctioneer equals auctioneerUser.Id
                        into auctioneerJoin
                    from auctioneer in auctioneerJoin.DefaultIfEmpty() // Left join for Auctioneer
                    select new
                    {
                        auction,
                        category,
                        createdUser,
                        updatedUser,
                        auctioneer,
                    };

                // Filter by AuctionName if provided (case-insensitive)
                if (!string.IsNullOrEmpty(getListAuctionRequest.AuctionName))
                {
                    query = query.Where(a =>
                        a.auction.AuctionName.ToLower()
                            .Contains(getListAuctionRequest.AuctionName.ToLower())
                    );
                }

                // Filter by CategoryId if provided
                if (getListAuctionRequest.CategoryId.HasValue)
                {
                    query = query.Where(a =>
                        a.auction.CategoryId == getListAuctionRequest.CategoryId.Value
                    );
                }

                // Filter by Status if provided
                if (getListAuctionRequest.Status.HasValue)
                {
                    query = query.Where(a =>
                        a.auction.Status == getListAuctionRequest.Status.Value
                    );
                }
                // Filter by ConditionAuction
                if (getListAuctionRequest.ConditionAuction.HasValue)
                {
                    if (getListAuctionRequest.ConditionAuction == 1)
                    {
                        query = query.Where(a =>
                            a.auction.RegisterOpenDate <= DateTime.Now
                            && a.auction.RegisterEndDate >= DateTime.Now
                        );
                    }
                    else if (getListAuctionRequest.ConditionAuction == 2)
                    {
                        query = query.Where(a =>
                            a.auction.RegisterEndDate < DateTime.Now
                            && a.auction.AuctionStartDate > DateTime.Now
                        );
                    }
                    else if (getListAuctionRequest.ConditionAuction == 3)
                    {
                        query = query.Where(a =>
                            a.auction.AuctionStartDate <= DateTime.Now
                            && a.auction.AuctionEndDate >= DateTime.Now
                        );
                    }
                    else if (getListAuctionRequest.ConditionAuction == 4)
                    {
                        query = query.Where(a => a.auction.AuctionEndDate < DateTime.Now);
                    }
                }

                // Filter by userId if provided
                if (!string.IsNullOrEmpty(userId))
                {
                    query = query.Where(a => a.auction.Auctioneer == Guid.Parse(userId));
                }

                // Calculate total count before pagination
                int totalCount = await query.CountAsync();

                // Sort by specified field
                if (!string.IsNullOrEmpty(getListAuctionRequest.SortBy))
                {
                    switch (getListAuctionRequest.SortBy.ToLower())
                    {
                        case "status":
                            query = getListAuctionRequest.IsAscending
                                ? query.OrderBy(a => a.auction.Status)
                                : query.OrderByDescending(a => a.auction.Status);
                            break;

                        case "auction_name":
                            query = getListAuctionRequest.IsAscending
                                ? query.OrderBy(a => a.auction.AuctionName)
                                : query.OrderByDescending(a => a.auction.AuctionName);
                            break;
                        case "register_open_date":
                            query = getListAuctionRequest.IsAscending
                                ? query.OrderBy(a => a.auction.RegisterOpenDate)
                                : query.OrderByDescending(a => a.auction.RegisterOpenDate);
                            break;
                        case "register_end_date":
                            query = getListAuctionRequest.IsAscending
                                ? query.OrderBy(a => a.auction.RegisterEndDate)
                                : query.OrderByDescending(a => a.auction.RegisterEndDate);
                            break;
                        case "auction_start_date":
                            query = getListAuctionRequest.IsAscending
                                ? query.OrderBy(a => a.auction.AuctionStartDate)
                                : query.OrderByDescending(a => a.auction.AuctionStartDate);
                            break;
                        case "auction_end_date":
                            query = getListAuctionRequest.IsAscending
                                ? query.OrderBy(a => a.auction.AuctionEndDate)
                                : query.OrderByDescending(a => a.auction.AuctionEndDate);
                            break;
                        case "created_by_username":
                            query = getListAuctionRequest.IsAscending
                                ? query.OrderBy(a => a.createdUser.Name)
                                : query.OrderByDescending(a => a.createdUser.Name);
                            break;
                        case "update_by_username":
                            query = getListAuctionRequest.IsAscending
                                ? query
                                    .OrderBy(a => a.updatedUser != null ? a.updatedUser.Name : null)
                                    .ThenBy(a => a.auction.AuctionId)
                                : query
                                    .OrderByDescending(a =>
                                        a.updatedUser != null ? a.updatedUser.Name : null
                                    )
                                    .ThenByDescending(a => a.auction.AuctionId);
                            break;
                        case "auctioneer_by":
                            query = getListAuctionRequest.IsAscending
                                ? query.OrderBy(a =>
                                    a.auctioneer != null ? a.auctioneer.Name : null
                                )
                                : query.OrderByDescending(a =>
                                    a.auctioneer != null ? a.auctioneer.Name : null
                                );
                            break;
                        default:
                            query = query.OrderBy(a => a.auction.CreatedAt); // Default sort by CreatedAt
                            break;
                    }
                }
                else
                {
                    query = query.OrderBy(a => a.auction.CreatedAt); // Default sort by CreatedAt
                }

                // Apply pagination
                var pageNumber = getListAuctionRequest.PageNumber ?? 1;
                var pageSize = getListAuctionRequest.PageSize ?? 2;

                query = query.Skip((pageNumber - 1) * pageSize).Take(pageSize);

                // Map to ListAuctionDTO
                var auctions = await query
                    .Select(a => new ListAuctionDTO
                    {
                        AuctionId = a.auction.AuctionId,
                        AuctionName = a.auction.AuctionName,
                        CategoryId = a.auction.CategoryId,
                        Status = a.auction.Status,
                        RegisterOpenDate = a.auction.RegisterOpenDate,
                        RegisterEndDate = a.auction.RegisterEndDate,
                        AuctionStartDate = a.auction.AuctionStartDate,
                        AuctionEndDate = a.auction.AuctionEndDate,
                        CreatedByUserName = a.createdUser.Name,
                        UpdateByUserName = a.updatedUser.Name,
                        AuctioneerBy = a.auctioneer != null ? a.auctioneer.Name : null,
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
                throw new Exception("Đã xảy ra lỗi khi lấy danh sách phiên đấu giá.", ex);
            }
        }
    }
}
