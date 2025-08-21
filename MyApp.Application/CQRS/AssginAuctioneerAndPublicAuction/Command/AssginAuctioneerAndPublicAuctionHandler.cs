using Hangfire;
using MediatR;
using MyApp.Application.Common.CurrentUserService;
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
        private readonly ICurrentUserService _currentUserService;
        private readonly ISetAuctionUpdateableFalse _setAuctionUpdateableFalse;
        private readonly INotificationSender _notificationSender;

        public AssginAuctioneerAndPublicAuctionHandler(
            IAssginAuctioneerAndPublicAuctionRepository repository,
            ICurrentUserService currentUserService,
            ISetAuctionUpdateableFalse setAuctionUpdateableFalse,
            INotificationSender notificationSender
        )
        {
            _repository = repository;
            _currentUserService = currentUserService;
            _setAuctionUpdateableFalse = setAuctionUpdateableFalse;
            _notificationSender = notificationSender;
        }

        public async Task<AssginAuctioneerAndPublicAuctionResponse> Handle(
            AssginAuctioneerAndPublicAuctionRequest request,
            CancellationToken cancellationToken
        )
        {
            var userId = _currentUserService.GetUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return new AssginAuctioneerAndPublicAuctionResponse
                {
                    Code = 401,
                    Message = Message.UNAUTHORIZED,
                };
            }

            // check status Auction is waiting
            var checkStatusAuction = await _repository.CheckStatusAuctionIsWaitingAsync(
                request.AuctionId
            );
            if (!checkStatusAuction)
            {
                return new AssginAuctioneerAndPublicAuctionResponse
                {
                    Code = 400,
                    Message = Message.AUCTION_NOT_WAITING,
                };
            }

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
            // convert list staff in charge to string
            string staffInCharges = string.Join(",", request.StaffInCharges);

            // assign auctioneer to auction and public auction
            var result = await _repository.AssignAuctioneerToAuctionAndPublicAuctionAsync(
                request.AuctionId,
                request.Auctioneer,
                userId,
                staffInCharges
            );

            if (!result.Item1)
            {
                return new AssginAuctioneerAndPublicAuctionResponse
                {
                    Code = 500,
                    Message = Message.SYSTEM_ERROR,
                };
            }

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
                await _setAuctionUpdateableFalse.SetAuctionUpdateableFalseAsync(request.AuctionId);
            }

            return new AssginAuctioneerAndPublicAuctionResponse
            {
                Code = 200,
                Message = Message.ASSGIN_AUCTIONEER_AND_PUBLIC_AUCTION_SUCCESS,
            };
        }
    }
}
