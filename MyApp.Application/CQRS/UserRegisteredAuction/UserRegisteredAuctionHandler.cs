using MediatR;
using MyApp.Application.Common.Message;
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
            var user = await _repository.GetUserByCitizenIdAsync(request.CitizenIdentification);
            if (user == null)
            {
                return new UserRegisteredAuctionResponseDTO
                {
                    Code = 404,
                    Message = Message.CITIZEN_NOT_EXIST,
                    Data = null,
                };
            }
            var auctionAssets = await _repository.GetValidAuctionAssetsAsync(
                user.Id,
                request.AuctionId
            );
            if (!auctionAssets.Any())
            {
                return new UserRegisteredAuctionResponseDTO
                {
                    Code = 404,
                    Message = Message.USER_NOT_REGISTERED_OR_INELIGIBLE_PARTICIPATE,
                    Data = null,
                };
            }
            if (request.AuctionRoundId == null)
            {
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
            else
            {
                // lấy danh sách tagname của người đấu giá đã đăng ký mà phải tiếp tục đấu giá ở vòng tiếp theo
                List<string> nextRoundTagNames = await _repository.GetNextRoundTagNamesForUserAsync(
                    request.AuctionRoundId,
                    request.CitizenIdentification
                );
                if (!nextRoundTagNames.Any())
                {
                    return new UserRegisteredAuctionResponseDTO
                    {
                        Code = 404,
                        Message = Message.USER_NOT_REGISTERED_OR_INELIGIBLE_PARTICIPATE,
                        Data = null,
                    };
                }
                auctionAssets = auctionAssets
                    .Where(a => nextRoundTagNames.Contains(a.TagName))
                    .ToList();
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
}
