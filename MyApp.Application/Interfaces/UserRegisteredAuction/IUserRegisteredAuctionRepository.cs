using MyApp.Application.CQRS.UserRegisteredAuction;

namespace MyApp.Application.Interfaces.UserRegisteredAuction
{
    public interface IUserRegisteredAuctionRepository
    {
        Task<UserRegisteredAuctionResponseDTO> UserRegisteredAuction(
            UserRegisteredAuctionRequest request
        );
    }
}
