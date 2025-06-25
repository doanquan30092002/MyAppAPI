using MediatR;
using MyApp.Application.Interfaces.RegisterAuctionDocument;

namespace MyApp.Application.CQRS.RegisterAuctionDocument.UpdateStatusTicket
{
    public class UpdateStatusTicketHandler : IRequestHandler<UpdateStatusTicketRequest, bool>
    {
        private readonly IRegisterAuctionDocumentRepository _registerAuctionDocumentRepository;

        public UpdateStatusTicketHandler(IRegisterAuctionDocumentRepository repository)
        {
            _registerAuctionDocumentRepository = repository;
        }

        public Task<bool> Handle(
            UpdateStatusTicketRequest request,
            CancellationToken cancellationToken
        )
        {
            var result = _registerAuctionDocumentRepository.UpdateStatusTicketAsync(
                request.AuctionDocumentsId
            );
            return result;
        }
    }
}
