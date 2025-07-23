using DocumentFormat.OpenXml.Spreadsheet;
using MediatR;
using MyApp.Application.Common.Message;
using MyApp.Application.Common.Services.NotificationHub;
using MyApp.Application.Interfaces.RegisterAuctionDocument.Repository;

namespace MyApp.Application.CQRS.RegisterAuctionDocument.UpdateStatusTicket
{
    public class UpdateStatusTicketHandler : IRequestHandler<UpdateStatusTicketRequest, bool>
    {
        private readonly IRegisterAuctionDocumentRepository _repository;
        private readonly INotificationSender _notificationSender;

        public UpdateStatusTicketHandler(
            IRegisterAuctionDocumentRepository registerAuctionDocumentRepository,
            INotificationSender notificationSender
        )
        {
            _repository = registerAuctionDocumentRepository;
            _notificationSender = notificationSender;
        }

        public async Task<bool> Handle(
            UpdateStatusTicketRequest request,
            CancellationToken cancellationToken
        )
        {
            var result = await _repository.UpdateStatusTicketAndGetUserIdAsync(
                request.AuctionDocumentsId
            );
            if (result)
            {
                // get userId from the repository with role Staff = 3
                List<Guid> userIdStaff = await _repository.GetUserIdByRoleAsync();
                // get auction name by auctionDocumentsId
                string auctionName = await _repository.GetAuctionNameByAuctionDocumentsIdAsync(
                    request.AuctionDocumentsId
                );
                //send notification to all staff
                var message = string.Format(Message.NEW_AUCTION_TO_CUSTOMER, auctionName);
                await _notificationSender.SendToUsersAsync(userIdStaff, message);
                // save notification to database
                await _repository.SaveNotificationAsync(userIdStaff, message);
            }
            return result;
        }
    }
}
