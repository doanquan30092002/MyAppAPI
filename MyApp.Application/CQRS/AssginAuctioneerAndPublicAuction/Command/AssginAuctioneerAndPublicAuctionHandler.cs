using Hangfire;
using MediatR;
using MyApp.Application.Common.CurrentUserService;
using MyApp.Application.Common.Message;
using MyApp.Application.Common.Services.NotificationHub;
using MyApp.Application.Common.Services.SendMessage;
using MyApp.Application.Common.Utils;
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
        private readonly ITemplateEmail _templateEmail;

        public AssginAuctioneerAndPublicAuctionHandler(
            IAssginAuctioneerAndPublicAuctionRepository repository,
            ICurrentUserService currentUserService,
            ISetAuctionUpdateableFalse setAuctionUpdateableFalse,
            INotificationSender notificationSender,
            IEnumerable<ISendMessage> sendMessages,
            ITemplateEmail templateEmail
        )
        {
            _repository = repository;
            _currentUserService = currentUserService;
            _setAuctionUpdateableFalse = setAuctionUpdateableFalse;
            _notificationSender = notificationSender;
            _sendMessages = sendMessages;
            _templateEmail = templateEmail;
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
                    $"[Thông báo phân công] Bạn được giao nhiệm vụ hỗ trợ phiên đấu giá {result.Item3}";

                var content_auctioneer = _templateEmail.BuildEmailHtml(
                    "Phân công Đấu giá viên",
                    $@"<p style=""margin:0 0 12px 0;"">
          Bạn đã được phân công làm <strong>Đấu giá viên</strong> cho phiên đấu giá <strong>{result.Item3}</strong>.
       </p>
       <p style=""margin:0;"">
          Vui lòng truy cập hệ thống để xem chi tiết và chuẩn bị điều hành.
       </p>"
                );

                var content_staff = _templateEmail.BuildEmailHtml(
                    "Phân công hỗ trợ phiên đấu giá",
                    $@"<p style=""margin:0 0 12px 0;"">
          Bạn đã được phân công <strong>hỗ trợ</strong> cho phiên đấu giá <strong>{result.Item3}</strong>.
       </p>
       <p style=""margin:0;"">
          Vui lòng truy cập hệ thống để xem chi tiết.
       </p>"
                );

                // ----- Gửi email -----
                // Lưu ý: cần bật gửi dạng HTML trong phương thức SendAsync (ví dụ tham số isHtml=true)
                // Ví dụ giả định chữ ký (emailFrom, subject, htmlBody, attachments, isHtml)
                await emailSender.SendAsync(
                    emailFromAuctioneer,
                    subject_auctioneer,
                    content_auctioneer,
                    null
                );

                // Gửi cho danh sách staff
                await emailSender.SendAsync(
                    "", // from hoặc dùng emailFromStaff nếu có
                    subject_staff,
                    content_staff,
                    staffEmails
                );
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
