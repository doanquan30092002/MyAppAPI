using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MyApp.Application.Common.Message;
using MyApp.Application.CQRS.Auction.GetListAuction.Querries;
using MyApp.Application.CQRS.Auction.GetListAution.Querries;
using MyApp.Application.Interfaces.IGetListDocumentsRepository;
using MyApp.Core.Entities;
using MyApp.Infrastructure.Data;

namespace MyApp.Infrastructure.Repositories.GetListAuctionDocumentsRepository
{
    public class GetListAuctionDocumentsRepository : IGetListDocumentsRepository
    {
        private readonly AppDbContext context;

        public GetListAuctionDocumentsRepository(AppDbContext context)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<GetListAuctionDocumentsResponse> GetListAuctionDocumentsAsync(
            GetListAuctionDocumentsRequest getListAuctionDocumentsRequest
        )
        {
            try
            {
                var query = context
                    .AuctionDocuments.Include(a => a.User)
                    .Include(a => a.AuctionAsset)
                    .AsQueryable();

                query = query.Where(ad =>
                    ad.AuctionAsset.AuctionId == getListAuctionDocumentsRequest.AuctionId
                );

                // Filter by CitizenIdentification if provided
                if (!string.IsNullOrEmpty(getListAuctionDocumentsRequest.CitizenIdentification))
                {
                    query = query.Where(ad =>
                        ad.User != null
                        && ad.User.CitizenIdentification.ToLower()
                            .Contains(
                                getListAuctionDocumentsRequest.CitizenIdentification.ToLower()
                            )
                    );
                }

                // Filter by Name if provided
                if (!string.IsNullOrEmpty(getListAuctionDocumentsRequest.Name))
                {
                    query = query.Where(ad =>
                        ad.User != null
                        && ad.User.Name.ToLower()
                            .Contains(getListAuctionDocumentsRequest.Name.ToLower())
                    );
                }

                // Filter by TagName if provided
                if (!string.IsNullOrEmpty(getListAuctionDocumentsRequest.TagName))
                {
                    query = query.Where(ad =>
                        ad.AuctionAsset != null
                        && ad.AuctionAsset.TagName.ToLower()
                            .Contains(getListAuctionDocumentsRequest.TagName.ToLower())
                    );
                }

                // Filter by StatusTicket if provided
                if (getListAuctionDocumentsRequest.StatusTicket.HasValue)
                {
                    query = query.Where(ad =>
                        ad.StatusTicket == getListAuctionDocumentsRequest.StatusTicket.Value
                    );
                }

                // Filter by StatusDeposit if provided
                if (getListAuctionDocumentsRequest.StatusDeposit.HasValue)
                {
                    query = query.Where(ad =>
                        ad.StatusDeposit == getListAuctionDocumentsRequest.StatusDeposit.Value
                    );
                }

                // Calculate total count before pagination
                int totalCount = await query.CountAsync();

                // Group by AuctionAssetId and count documents for DocumentsAssetList
                var documentsAssetList = await query
                    .GroupBy(ad => ad.AuctionAssetId)
                    .Select(g => new DocumentsAssetDto { AssetId = g.Key, Quantity = g.Count() })
                    .ToListAsync();

                // Sort by specified field
                if (!string.IsNullOrEmpty(getListAuctionDocumentsRequest.SortBy))
                {
                    switch (getListAuctionDocumentsRequest.SortBy.ToLower())
                    {
                        case "citizenidentification":
                            query = getListAuctionDocumentsRequest.IsAscending
                                ? query.OrderBy(ad =>
                                    ad.User != null ? ad.User.CitizenIdentification : string.Empty
                                )
                                : query.OrderByDescending(ad =>
                                    ad.User != null ? ad.User.CitizenIdentification : string.Empty
                                );
                            break;
                        case "name":
                            query = getListAuctionDocumentsRequest.IsAscending
                                ? query.OrderBy(ad => ad.User != null ? ad.User.Name : string.Empty)
                                : query.OrderByDescending(ad =>
                                    ad.User != null ? ad.User.Name : string.Empty
                                );
                            break;
                        case "tagname":
                            query = getListAuctionDocumentsRequest.IsAscending
                                ? query.OrderBy(ad =>
                                    ad.AuctionAsset != null ? ad.AuctionAsset.TagName : string.Empty
                                )
                                : query.OrderByDescending(ad =>
                                    ad.AuctionAsset != null ? ad.AuctionAsset.TagName : string.Empty
                                );
                            break;
                        case "deposit":
                            query = getListAuctionDocumentsRequest.IsAscending
                                ? query.OrderBy(ad =>
                                    ad.AuctionAsset != null ? ad.AuctionAsset.Deposit : 0
                                )
                                : query.OrderByDescending(ad =>
                                    ad.AuctionAsset != null ? ad.AuctionAsset.Deposit : 0
                                );
                            break;
                        case "statusticket":
                            query = getListAuctionDocumentsRequest.IsAscending
                                ? query.OrderBy(ad => ad.StatusTicket)
                                : query.OrderByDescending(ad => ad.StatusTicket);
                            break;
                        case "numericalorder":
                            query = getListAuctionDocumentsRequest.IsAscending
                                ? query.OrderBy(ad => ad.NumericalOrder)
                                : query.OrderByDescending(ad => ad.NumericalOrder);
                            break;
                        default:
                            query = query.OrderBy(ad =>
                                ad.User != null ? ad.User.Name : string.Empty
                            );
                            break;
                    }
                }
                else
                {
                    query = query.OrderBy(ad => ad.User != null ? ad.User.Name : string.Empty);
                }

                // Apply pagination
                var pageNumber = getListAuctionDocumentsRequest.PageNumber ?? 1;
                var pageSize = getListAuctionDocumentsRequest.PageSize ?? 2;

                query = query.Skip((pageNumber - 1) * pageSize).Take(pageSize);

                // Map to ListAuctionDocumentsDTO
                var auctionDocuments = await query
                    .Select(ad => new ListAuctionDocumentsDTO
                    {
                        AuctionDocumentsId = ad.AuctionDocumentsId,
                        CitizenIdentification =
                            ad.User != null ? ad.User.CitizenIdentification : string.Empty,
                        Name = ad.User != null ? ad.User.Name : string.Empty,
                        TagName = ad.AuctionAsset != null ? ad.AuctionAsset.TagName : string.Empty,
                        Deposit = ad.AuctionAsset != null ? ad.AuctionAsset.Deposit : 0,
                        StatusDeposit = ad.StatusDeposit,
                        RegistrationFee =
                            ad.AuctionAsset != null ? ad.AuctionAsset.RegistrationFee : 0,
                        StatusTicket = ad.StatusTicket,
                        IsAttended = ad.IsAttended ?? false,
                        StatusRefund = ad.StatusRefund ?? 0,
                        RefundReason = ad.RefundReason ?? string.Empty,
                        RefundProof = ad.RefundProof ?? string.Empty,
                        NumericalOrder = ad.NumericalOrder,
                        Note = ad.Note,
                    })
                    .ToListAsync();

                // Create response
                var response = new GetListAuctionDocumentsResponse
                {
                    TotalCount = totalCount,
                    DocumentsAssetList = documentsAssetList,
                    AuctionDocuments = auctionDocuments,
                };

                return response;
            }
            catch (Exception ex)
            {
                throw new Exception(Message.GET_LIST_AUCTION_DOCUMENT_FAIL, ex);
            }
        }
    }
}
