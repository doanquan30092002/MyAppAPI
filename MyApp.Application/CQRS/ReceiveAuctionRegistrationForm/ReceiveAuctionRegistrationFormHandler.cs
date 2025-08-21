using MediatR;
using MyApp.Application.Common.CurrentUserService;
using MyApp.Application.Common.Message;
using MyApp.Application.Common.Services.NotificationHub;
using MyApp.Application.Interfaces.ReceiveAuctionRegistrationForm;

namespace MyApp.Application.CQRS.ReceiveAuctionRegistrationForm
{
    public class ReceiveAuctionRegistrationFormHandler
        : IRequestHandler<ReceiveAuctionRegistrationFormRequest, bool>
    {
        private readonly IReceiveAuctionRegistrationFormRepository _repository;
        private readonly INotificationSender _notificationSender;

        public ReceiveAuctionRegistrationFormHandler(
            IReceiveAuctionRegistrationFormRepository repository,
            INotificationSender notificationSender
        )
        {
            _repository = repository;
            _notificationSender = notificationSender;
        }

        public async Task<bool> Handle(
            ReceiveAuctionRegistrationFormRequest request,
            CancellationToken cancellationToken
        )
        {
            var response = await _repository.UpdateStatusTicketSigned(
                request.AuctionDocumentsId,
                request.StatusTicket,
                request.Note
            );
            if (response)
            {
                // get userId of the auction document
                List<Guid> userId = await _repository.GetUserIdByAuctionDocumentId(
                    request.AuctionDocumentsId
                );
                // get auction name by auctionDocumentsId
                string auctionName = await _repository.GetAuctionNameByAuctionDocumentsIdAsync(
                    request.AuctionDocumentsId
                );
                // save notification to the database
                var message = string.Format(Message.RECEIVED_FORM_SUCCESS, auctionName);
                ;
                await _notificationSender.SendToUsersAsync(userId, message);
                // send notification to the user
                await _repository.SaveNotificationAsync(userId, message);
            }
            return response;
        }
    }
}
