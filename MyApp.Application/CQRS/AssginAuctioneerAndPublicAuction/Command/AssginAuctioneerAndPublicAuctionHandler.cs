using Hangfire;
using MediatR;
using MyApp.Application.Common.CurrentUserService;
using MyApp.Application.Common.Message;
using MyApp.Application.Common.Services.NotificationHub;
using MyApp.Application.Common.Services.SendMessage;
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
        private readonly IEnumerable<ISendMessage> _sendMessages;

        public AssginAuctioneerAndPublicAuctionHandler(
            IAssginAuctioneerAndPublicAuctionRepository repository,
            ICurrentUserService currentUserService,
            ISetAuctionUpdateableFalse setAuctionUpdateableFalse,
            INotificationSender notificationSender,
            IEnumerable<ISendMessage> sendMessages
        )
        {
            _repository = repository;
            _currentUserService = currentUserService;
            _setAuctionUpdateableFalse = setAuctionUpdateableFalse;
            _notificationSender = notificationSender;
            _sendMessages = sendMessages;
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
            //get list email from staff in charge
            List<string> staffEmails = await _repository.GetEmailFromStaffInCharges(
                request.StaffInCharges
            );
            //get email sender from Auctioneer
            string emailFromAuctioneer = await _repository.GetEmailFromAuctioneer(
                request.Auctioneer
            );
            // get userId role customer
            List<Guid> userIds = await _repository.GetAllUserIdRoleCustomer();

            // send notification to customer
            var message = string.Format(Message.NEW_AUCTION_TO_CUSTOMER, result.Item3);
            await _notificationSender.SendToUsersAsync(userIds, new { Message = message });

            //send mail to auctioneer and staff in charge
            var emailSender = _sendMessages.FirstOrDefault(x =>
                x.Channel == SendMessageChannel.Email
            );

            if (emailSender != null && request.StaffInCharges.Any())
            {
                var subject_auctioneer =
                    $"[Thông báo phân công] Bạn được chỉ định làm Đấu giá viên cho phiên {result.Item3}";
                var subject_staff =
                    $"[Thông báo phân công]: Bạn được giao nhiệm vụ hỗ trợ phiên đấu giá {result.Item3}";
                var content_auctioneer =
                    $"Bạn đã được phân công làm Đấu giá viên cho phiên đấu giá {result.Item3}.\r\n\r\nVui lòng truy cập hệ thống để xem chi tiết và chuẩn bị điều hành.\r\n\r\nTrân trọng,  \r\n[Hệ thống đấu giá số Tuấn Linh]";
                var content_staff =
                    $"Bạn đã được phân công hỗ trợ cho phiên đấu giá {result.Item3}.\r\n\r\nVui lòng truy cập hệ thống để xem chi tiết.\r\n\r\nTrân trọng,  \r\n[Hệ thống đấu giá số Tuấn Linh]";
                await emailSender.SendAsync(
                    emailFromAuctioneer,
                    subject_auctioneer,
                    content_auctioneer,
                    null
                );
                await emailSender.SendAsync("", subject_staff, content_staff, staffEmails);
            }

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
