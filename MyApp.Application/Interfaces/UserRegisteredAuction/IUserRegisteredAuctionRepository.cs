using MyApp.Application.CQRS.UserRegisteredAuction;
using MyApp.Core.Entities;

namespace MyApp.Application.Interfaces.UserRegisteredAuction
{
    public interface IUserRegisteredAuctionRepository
    {
        Task<string> GetCitizenIdentificationByAuctionIdAndNumericalOrderAsync(
            Guid auctionId,
            int numericalOrder
        );
        Task<List<(string TagName, decimal AuctionPrice)>> GetNextRoundTagNamesForUserAsync(
            Guid? auctionRoundId,
            string citizenIdentification
        );
        Task<User?> GetUserByCitizenIdAsync(string citizenId);
        Task<List<AuctionAsset>> GetValidAuctionAssetsAsync(Guid userId, Guid auctionId);
    }
}
