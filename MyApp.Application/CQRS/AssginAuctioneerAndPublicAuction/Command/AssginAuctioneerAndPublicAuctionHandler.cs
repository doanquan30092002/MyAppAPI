using System.Security.Claims;
using Hangfire;
using MediatR;
using Microsoft.AspNetCore.Http;
using MyApp.Application.Common.Message;
using MyApp.Application.Common.Services.NotificationHub;
using MyApp.Application.Interfaces.AssginAuctioneerAndPublicAuction;
using MyApp.Application.JobBackgroud.AuctionJob;

namespace MyApp.Application.CQRS.AssginAuctioneerAndPublicAuction.Command
{
    public class AssginAuctioneerAndPublicAuctionHandler
        : IRequestHandler<
            AssginAuctioneerAndPublicAuctionRequest,
            AssginAuctioneerAndPublicAuctionResponse
        >
    {
        private readonly IAssginAuctioneerAndPublicAuctionRepository _repository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly SetAuctionUpdateableFalse _setAuctionUpdateableFalse;
        private readonly INotificationSender _notificationSender;

        public AssginAuctioneerAndPublicAuctionHandler(
            IAssginAuctioneerAndPublicAuctionRepository repository,
            IHttpContextAccessor httpContextAccessor,
            SetAuctionUpdateableFalse setAuctionUpdateableFalse,
            INotificationSender notificationSender
        )
        {
            _repository = repository;
            _httpContextAccessor = httpContextAccessor;
            _setAuctionUpdateableFalse = setAuctionUpdateableFalse;
            _notificationSender = notificationSender;
        }

        public async Task<AssginAuctioneerAndPublicAuctionResponse> Handle(
            AssginAuctioneerAndPublicAuctionRequest request,
            CancellationToken cancellationToken
        )
        {
            string userId = _httpContextAccessor
                .HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)
                ?.Value;
            // check 1 auctioneer cannot be assigned to 2 auctions at the same time
            var checkAuctioneerAssigned =
                await _repository.CheckAuctioneerAssignedToAnotherAuctionAsync(
                    request.Auctioneer,
                    request.AuctionId
                );
            if (checkAuctioneerAssigned)
            {
                return new AssginAuctioneerAndPublicAuctionResponse
                {
                    Code = 400,
                    Message = Message.AUCTIONEER_ASSIGNED_ANOTHER_AUCTION,
                };
            }
            // assign auctioneer to auction and public auction
            var result = await _repository.AssignAuctioneerToAuctionAndPublicAuctionAsync(
                request.AuctionId,
                request.Auctioneer,
                userId
            );
            if (result.Item1)
            {
                // get userId role customer
                List<Guid> userIds = await _repository.GetAllUserIdRoleCustomer();
                // send notification to customer
                var message = string.Format(Message.NEW_AUCTION_TO_CUSTOMER, result.Item3);
                await _notificationSender.SendToUsersAsync(userIds, new { Message = message });
                // save notification to database
                var checkSaveNotificationAsync = await _repository.SaveNotificationAsync(
                    userIds,
                    message
                );
                if (!checkSaveNotificationAsync)
                {
                    return new AssginAuctioneerAndPublicAuctionResponse
                    {
                        Code = 500,
                        Message = Message.SYSTEM_ERROR,
                    };
                }
                // Set Auction Updateable False
                var delaySetAuctionUpdateableFalse = DateTime.Parse(result.Item2) - DateTime.Now;
                if (delaySetAuctionUpdateableFalse > TimeSpan.Zero)
                {
                    BackgroundJob.Schedule<SetAuctionUpdateableFalse>(
                        job => job.SetAuctionUpdateableFalseAsync(request.AuctionId),
                        delaySetAuctionUpdateableFalse
                    );
                }
                else
                {
                    await _setAuctionUpdateableFalse.SetAuctionUpdateableFalseAsync(
                        request.AuctionId
                    );
                }

                return new AssginAuctioneerAndPublicAuctionResponse
                {
                    Code = 200,
                    Message = Message.ASSGIN_AUCTIONEER_AND_PUBLIC_AUCTION_SUCCESS,
                };
            }
            else
            {
                return new AssginAuctioneerAndPublicAuctionResponse
                {
                    Code = 500,
                    Message = Message.SYSTEM_ERROR,
                };
            }
        }
    }
}
