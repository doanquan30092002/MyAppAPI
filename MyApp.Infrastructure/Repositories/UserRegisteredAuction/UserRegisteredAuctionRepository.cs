using Microsoft.EntityFrameworkCore;
using MyApp.Application.Common.Message;
using MyApp.Application.CQRS.UserRegisteredAuction;
using MyApp.Application.Interfaces.UserRegisteredAuction;
using MyApp.Infrastructure.Data;

namespace MyApp.Infrastructure.Repositories.UserRegisteredAuction
{
    public class UserRegisteredAuctionRepository : IUserRegisteredAuctionRepository
    {
        private readonly AppDbContext _context;

        public UserRegisteredAuctionRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<UserRegisteredAuctionResponseDTO> UserRegisteredAuction(
            UserRegisteredAuctionRequest request
        )
        {
            // 1. Find the user by CitizenIdentification
            var user = await _context.Users.FirstOrDefaultAsync(u =>
                u.CitizenIdentification == request.CitizenIdentification
            );

            if (user == null)
                return new UserRegisteredAuctionResponseDTO
                {
                    Code = 404,
                    Message = Message.CITIZEN_NOT_EXIST,
                    Data = null,
                };

            // 2. Find all AuctionDocuments for this user and auction
            var auctionAssets = await _context
                .AuctionDocuments.Where(ad =>
                    ad.UserId == user.Id
                    && ad.AuctionAsset.AuctionId == request.AuctionId
                    && ad.StatusTicket == 2
                    && ad.StatusDeposit == 1
                )
                .Select(ad => new AuctionAsset
                {
                    AuctionAssetsId = ad.AuctionAsset.AuctionAssetsId,
                    TagName = ad.AuctionAsset.TagName,
                })
                .ToListAsync();

            if (!auctionAssets.Any())
                return new UserRegisteredAuctionResponseDTO
                {
                    Code = 404,
                    Message = Message.USER_NOT_REGISTERED_OR_INELIGIBLE_PARTICIPATE,
                    Data = null,
                };

            // 3. Build and return the response
            return new UserRegisteredAuctionResponseDTO
            {
                Code = 200,
                Message = Message.GET_USER_REGISTERED_AUCTION_SUCCESS,
                Data = new UserRegisteredAuctionResponse
                {
                    Id = user.Id,
                    Name = user.Name,
                    CitizenIdentification = user.CitizenIdentification,
                    RecentLocation = user.RecentLocation,
                    AuctionAssets = auctionAssets,
                },
            };
        }
    }
}
