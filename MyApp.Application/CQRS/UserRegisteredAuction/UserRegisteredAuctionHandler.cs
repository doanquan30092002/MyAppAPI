using MediatR;
using MyApp.Application.Interfaces.UserRegisteredAuction;

namespace MyApp.Application.CQRS.UserRegisteredAuction
{
    public class UserRegisteredAuctionHandler
        : IRequestHandler<UserRegisteredAuctionRequest, UserRegisteredAuctionResponseDTO>
    {
        private readonly IUserRegisteredAuctionRepository _repository;

        public UserRegisteredAuctionHandler(IUserRegisteredAuctionRepository repository)
        {
            _repository = repository;
        }

        public async Task<UserRegisteredAuctionResponseDTO> Handle(
            UserRegisteredAuctionRequest request,
            CancellationToken cancellationToken
        )
        {
            UserRegisteredAuctionResponseDTO response = await _repository.UserRegisteredAuction(
                request
            );
            return response;
        }
    }
}
