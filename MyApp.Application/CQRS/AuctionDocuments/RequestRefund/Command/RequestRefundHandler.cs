using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using MyApp.Application.Common.Services.UploadFile;
using MyApp.Application.Interfaces.IAuctionDocuments;

namespace MyApp.Application.CQRS.AuctionDocuments.RequestRefund.Command
{
    public class RequestRefundHandler : IRequestHandler<RequestRefundCommand, bool>
    {
        private readonly IAuctionDocuments _auctionDocumentsRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUploadFile _uploadFileService;

        public RequestRefundHandler(
            IAuctionDocuments auctionDocumentsRepository,
            IHttpContextAccessor httpContextAccessor,
            IUploadFile uploadFileService
        )
        {
            _auctionDocumentsRepository = auctionDocumentsRepository;
            _httpContextAccessor = httpContextAccessor;
            _uploadFileService = uploadFileService;
        }

        public async Task<bool> Handle(
            RequestRefundCommand request,
            CancellationToken cancellationToken
        )
        {
            Guid? userId = null;
            var userIdStr = _httpContextAccessor
                .HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)
                ?.Value;
            if (!string.IsNullOrEmpty(userIdStr) && Guid.TryParse(userIdStr, out var parsedUserId))
            {
                userId = parsedUserId;
            }
            else
            {
                throw new UnauthorizedAccessException("Không thể xác định người dùng hiện tại.");
            }

            var currentTime = DateTime.Now;

            foreach (var documentId in request.AuctionDocumentIds)
            {
                var document = await _auctionDocumentsRepository.GetDocumentByIdAndUserIdAsync(
                    documentId,
                    userId.Value
                );
                if (document == null)
                {
                    throw new UnauthorizedAccessException(
                        $"Hồ sơ với ID {documentId} không thuộc về người dùng hiện tại hoặc không tồn tại."
                    );
                }

                var auction = await _auctionDocumentsRepository.GetAuctionByAuctionDocumentIdAsync(
                    documentId
                );
                if (auction == null)
                {
                    throw new InvalidOperationException(
                        $"Không tìm thấy phiên đấu giá cho hồ sơ ID {documentId}."
                    );
                }

                if (currentTime >= auction.AuctionStartDate)
                {
                    throw new InvalidOperationException(
                        $"Hồ sơ ID {documentId} không thể yêu cầu hoàn cọc do đã sau thời gian bắt đầu đấu giá."
                    );
                }

                if (document.StatusDeposit != 1)
                {
                    throw new InvalidOperationException(
                        $"Hồ sơ với ID {documentId} không đủ điều kiện để yêu cầu hoàn tiền: Chưa nộp tiền cọc."
                    );
                }

                if (document.StatusRefund == 1)
                {
                    throw new InvalidOperationException(
                        $"Hồ sơ với ID {documentId} đã được yêu cầu hoàn tiền cọc trước đó."
                    );
                }

                if (document.StatusRefund == 2)
                {
                    throw new InvalidOperationException(
                        $"Hồ sơ với ID {documentId} không đủ điều kiện để yêu cầu hoàn tiền: Đã chấp nhận hoàn."
                    );
                }
            }

            var refundProofUrl = await _uploadFileService.UploadAsync(request.RefundProof);

            return await _auctionDocumentsRepository.RequestRefundAsync(
                request.AuctionDocumentIds,
                userId.Value,
                refundProofUrl,
                request.RefundReason
            );
        }
    }
}
