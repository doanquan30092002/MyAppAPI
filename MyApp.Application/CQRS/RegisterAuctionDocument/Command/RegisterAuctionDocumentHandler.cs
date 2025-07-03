using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Http;
using MyApp.Application.Common.Message;
using MyApp.Application.Interfaces.RegisterAuctionDocument.Repository;
using MyApp.Core.Entities;

namespace MyApp.Application.CQRS.RegisterAuctionDocument.Command
{
    public class RegisterAuctionDocumentHandler
        : IRequestHandler<RegisterAuctionDocumentRequest, RegisterAuctionDocumentResponse>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IRegisterAuctionDocumentRepository _repository;

        public RegisterAuctionDocumentHandler(
            IHttpContextAccessor httpContextAccessor,
            IRegisterAuctionDocumentRepository repository
        )
        {
            _httpContextAccessor = httpContextAccessor;
            _repository = repository;
        }

        public async Task<RegisterAuctionDocumentResponse> Handle(
            RegisterAuctionDocumentRequest request,
            CancellationToken cancellationToken
        )
        {
            var result = new RegisterAuctionDocumentResponse();
            string? userId = _httpContextAccessor
                .HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)
                ?.Value;
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
                    result = await _repository.CreateQRForPayTicket(
                        auctionDocument.AuctionDocumentsId
                    );
                }
            }

            return result;
        }
    }
}
