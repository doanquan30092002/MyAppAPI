using MediatR;
using MyApp.Application.Interfaces.RegisterAuctionDocument.Repository;
using MyApp.Application.Interfaces.RegisterAuctionDocument.Service;

namespace MyApp.Application.CQRS.RegisterAuctionDocument.UpdateStatusTicket
{
    public class UpdateStatusTicketHandler : IRequestHandler<UpdateStatusTicketRequest, bool>
    {
        private readonly IRegisterAuctionDocumentRepository _registerAuctionDocumentRepository;
        private readonly INotificationService _notificationService;

        public UpdateStatusTicketHandler(
            IRegisterAuctionDocumentRepository registerAuctionDocumentRepository,
            INotificationService notificationService
        )
        {
            _registerAuctionDocumentRepository = registerAuctionDocumentRepository;
            _notificationService = notificationService;
        }

        public async Task<bool> Handle(
            UpdateStatusTicketRequest request,
            CancellationToken cancellationToken
        )
        {
            var result =
                await _registerAuctionDocumentRepository.UpdateStatusTicketAndGetUserIdAsync(
                    request.AuctionDocumentsId
                );
            return result;
        }
    }
}
