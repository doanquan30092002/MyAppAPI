using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using MyApp.Application.Common.CurrentUserService;
using MyApp.Application.Common.Services.UploadFile;
using MyApp.Application.CQRS.AuctionDocuments.RequestRefund.Helper;
using MyApp.Application.Interfaces.IAuctionDocuments;

namespace MyApp.Application.CQRS.AuctionDocuments.RequestRefund.Command
{
    public class RequestRefundHandler : IRequestHandler<RequestRefundCommand, bool>
    {
        private readonly IAuctionDocuments _auctionDocumentsRepository;
        private readonly IRequestRefundHelper _helper;

        public RequestRefundHandler(
            IAuctionDocuments auctionDocumentsRepository,
            IRequestRefundHelper helper
        )
        {
            _auctionDocumentsRepository = auctionDocumentsRepository;
            _helper = helper;
        }

        public async Task<bool> Handle(
            RequestRefundCommand request,
            CancellationToken cancellationToken
        )
        {
            var userId = _helper.GetCurrentUserId();

            await Task.WhenAll(
                request.AuctionDocumentIds.Select(docId =>
                    _helper.ValidateDocumentForRefundAsync(docId, userId)
                )
            );

            var refundProofUrl = await _helper.UploadRefundProofAsync(request.RefundProof);

            return await _auctionDocumentsRepository.RequestRefundAsync(
                request.AuctionDocumentIds,
                userId,
                refundProofUrl,
                request.RefundReason
            );
        }
    }
}
