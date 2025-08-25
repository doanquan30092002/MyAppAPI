using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MyApp.Application.Common.Message;
using MyApp.Application.CQRS.Auction.GetListAuctionById.Querries;
using MyApp.Application.Interfaces.IGetAuctionByIdRepository;
using MyApp.Core.Entities;
using MyApp.Infrastructure.Data;

namespace MyApp.Infrastructure.Repositories.GetAuctionByIdRepository
{
    public class GetAuctionByIdRepository : IGetAuctionByIdRepository
    {
        private readonly AppDbContext context;

        public GetAuctionByIdRepository(AppDbContext context)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<GetAuctionByIdResponse> GetAuctionByIdAsync(Guid auctionId)
        {
            try
            {
                var auction = await (
                    from a in context.Auctions
                    join createdUser in context.Users
                        on a.CreatedBy equals createdUser.Id
                        into createdUsers
                    from createdUser in createdUsers.DefaultIfEmpty()
                    join updatedUser in context.Users
                        on a.UpdatedBy equals updatedUser.Id
                        into updatedUsers
                    from updatedUser in updatedUsers.DefaultIfEmpty()
                    join category in context.AuctionCategories
                        on a.CategoryId equals category.CategoryId
                        into categories
                    from category in categories.DefaultIfEmpty()
                    join auctioneerUser in context.Users
                        on a.Auctioneer equals auctioneerUser.Id
                        into auctioneerUsers
                    from auctioneerUser in auctioneerUsers.DefaultIfEmpty()
                    where a.AuctionId == auctionId
                    select new GetAuctionByIdResponse
                    {
                        AuctionName = a.AuctionName,
                        AuctionDescription = a.AuctionDescription,
                        AuctionRules = a.AuctionRules,
                        AuctionPlanningMap = a.AuctionPlanningMap,
                        RegisterOpenDate = a.RegisterOpenDate,
                        RegisterEndDate = a.RegisterEndDate,
                        AuctionStartDate = a.AuctionStartDate,
                        AuctionEndDate = a.AuctionEndDate,
                        AuctionMap = a.AuctionMap,
                        CreatedAt = a.CreatedAt,
                        CreatedByUserName = createdUser != null ? createdUser.Name : null,
                        UpdatedAt = a.UpdatedAt,
                        UpdatedByUserName = updatedUser != null ? updatedUser.Name : null,
                        QRLink = a.QRLink,
                        NumberRoundMax = a.NumberRoundMax,
                        Status = a.Status,
                        WinnerData = a.WinnerData,
                        CategoryName = category != null ? category.CategoryName : null,
                        CancelReasonFile = a.CancelReasonFile,
                        CancelReason = a.CancelReason,
                        RejectReason = a.RejectReason,
                        AuctioneerBy = auctioneerUser != null ? auctioneerUser.Name : null,
                        StaffInCharge = a.StaffInCharge,
                        ManagerInCharge = a.ManagerInCharge,
                        legalDocumentUrls = a.legalDocumentUrls,
                        PriceMin = a.PriceMin,
                        PriceMax = a.PriceMax,
                        ListAuctionAssets = (
                            from aa in context.AuctionAssets
                            where aa.AuctionId == a.AuctionId
                            select new AuctionAssets
                            {
                                AuctionAssetsId = aa.AuctionAssetsId,
                                TagName = aa.TagName,
                                StartingPrice = aa.StartingPrice,
                                Unit = aa.Unit,
                                Deposit = aa.Deposit,
                                RegistrationFee = aa.RegistrationFee,
                                Description = aa.Description,
                                CreatedAt = aa.CreatedAt,
                                CreatedBy = aa.CreatedBy,
                                UpdatedAt = aa.UpdatedAt,
                                UpdatedBy = aa.UpdatedBy,
                                AuctionId = aa.AuctionId,
                            }
                        ).ToList(),
                    }
                ).FirstOrDefaultAsync();

                // Check if auction exists
                if (auction == null)
                {
                    throw new Exception("Không tìm thấy phiên đấu giá");
                }

                return auction;
            }
            catch (Exception ex)
            {
                throw new Exception(Message.HANDLER_ERROR, ex);
            }
        }
    }
}
