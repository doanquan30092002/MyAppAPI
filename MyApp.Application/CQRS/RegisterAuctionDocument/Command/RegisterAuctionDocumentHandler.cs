using MediatR;
using MyApp.Application.Common.CurrentUserService;
using MyApp.Application.Common.Message;
using MyApp.Application.Interfaces.RegisterAuctionDocument.Repository;

namespace MyApp.Application.CQRS.RegisterAuctionDocument.Command
{
    public class RegisterAuctionDocumentHandler
        : IRequestHandler<RegisterAuctionDocumentRequest, RegisterAuctionDocumentResponse>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IRegisterAuctionDocumentRepository _repository;

        public RegisterAuctionDocumentHandler(
            ICurrentUserService currentUserService,
            IRegisterAuctionDocumentRepository repository
        )
        {
            _currentUserService = currentUserService;
            _repository = repository;
        }

        public async Task<RegisterAuctionDocumentResponse> Handle(
            RegisterAuctionDocumentRequest request,
            CancellationToken cancellationToken
        )
        {
            var result = new RegisterAuctionDocumentResponse();
            var userId = _currentUserService.GetUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return new RegisterAuctionDocumentResponse
                {
                    Code = 401,
                    Message = Message.UNAUTHORIZED,
                };
            }
            var auctionDocument = await _repository.CheckAuctionDocumentPaid(
                userId,
                request.AuctionAssetsId
            );
            if (auctionDocument == null)
            {
                Guid auctionDocumentId = await _repository.InsertAuctionDocumentAsync(
                    request.AuctionAssetsId,
                    userId,
                    request.BankAccount,
                    request.BankAccountNumber,
                    request.BankBranch
                );
                if (auctionDocumentId == Guid.Empty)
                    return new RegisterAuctionDocumentResponse
                    {
                        Code = 400,
                        Message = Message.REGISTER_AUCTION_DOCUMENT_FAIL,
                    };

                result = await _repository.CreateQRForPayTicket(auctionDocumentId);
            }
            else
            {
                if (auctionDocument.StatusTicket == Message.REGISTER_TICKET_PAID)
                {
                    return new RegisterAuctionDocumentResponse
                    {
                        Code = 400,
                        Message = Message.AUCTION_DOCUMENT_EXIST,
                    };
                }
                if (auctionDocument.StatusTicket == Message.REGISTER_TICKET_NOT_PAID)
                {
                    bool checkUpdate = await _repository.UpdateInforBankFromUser(
                        auctionDocument.AuctionDocumentsId,
                        request.BankAccount,
                        request.BankAccountNumber,
                        request.BankBranch
                    );
                    result = await _repository.CreateQRForPayTicket(
                        auctionDocument.AuctionDocumentsId
                    );
                }
            }

            return result;
        }
    }
}
