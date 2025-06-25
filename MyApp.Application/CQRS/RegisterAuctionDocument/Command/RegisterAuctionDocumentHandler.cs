using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Http;
using MyApp.Application.Common.Message;
using MyApp.Application.Interfaces.RegisterAuctionDocument;

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
            string? userId = _httpContextAccessor
                .HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)
                ?.Value;
            if (await _repository.CheckAuctionDocumentExsit(userId, request.AuctionAssetsId))
            {
                return new RegisterAuctionDocumentResponse
                {
                    Code = 400,
                    Message = Message.AUCTION_DOCUMENT_EXIST,
                };
            }
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

            var result = await _repository.CreateQRForPayTicket(auctionDocumentId);

            return result;
        }
    }
}
