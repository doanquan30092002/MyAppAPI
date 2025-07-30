namespace MyApp.Application.Interfaces.AssginAuctioneerAndPublicAuction
{
    public interface IAssginAuctioneerAndPublicAuctionRepository
    {
        // check 1 auctioneer cannot be assigned to 2 auctions at the same time
        Task<bool> CheckAuctioneerAssignedToAnotherAuctionAsync(Guid auctioneerId, Guid auctionId);

        // assign auctioneer to auction and public auction
        Task<(bool, string, string)> AssignAuctioneerToAuctionAndPublicAuctionAsync(
            Guid auctionId,
            Guid auctioneerId,
            string userId
        );
        Task<List<Guid>> GetAllUserIdRoleCustomer();
        Task<bool> SaveNotificationAsync(List<Guid> userIds, string message);
        Task<bool> CheckStatusAuctionIsWaitingAsync(Guid auctionId);
    }
}
