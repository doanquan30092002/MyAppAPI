using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using MyApp.Application.Common.CurrentUserService;
using MyApp.Application.Common.Services.UploadFile;
using MyApp.Application.Interfaces.IAuctionDocuments;

namespace MyApp.Application.CQRS.AuctionDocuments.RequestRefund.Helper
{
    public class RequestRefundHelper : IRequestRefundHelper
    {
        private readonly IAuctionDocuments _auctionDocumentsRepository;
        private readonly IUploadFile _uploadFileService;
        private readonly ICurrentUserService _currentUserService;

        public RequestRefundHelper(
            IAuctionDocuments auctionDocumentsRepository,
            IUploadFile uploadFileService,
            ICurrentUserService currentUserService
        )
        {
            _auctionDocumentsRepository = auctionDocumentsRepository;
            _uploadFileService = uploadFileService;
            _currentUserService = currentUserService;
        }

        public Guid GetCurrentUserId()
        {
            var userIdStr = _currentUserService.GetUserId();

            if (string.IsNullOrEmpty(userIdStr) || !Guid.TryParse(userIdStr, out var userId))
            {
                throw new UnauthorizedAccessException("Không thể xác định người dùng hiện tại.");
            }

            return userId;
        }

        public async Task ValidateDocumentForRefundAsync(Guid documentId, Guid userId)
        {
            var document = await _auctionDocumentsRepository.GetDocumentByIdAndUserIdAsync(
                documentId,
                userId
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

            if (DateTime.Now >= auction.AuctionStartDate)
            {
                throw new InvalidOperationException(
                    $"Hồ sơ ID {documentId} không thể yêu cầu hoàn cọc do đã sau thời gian bắt đầu đấu giá."
                );
            }

            switch (document.StatusRefund)
            {
                case 1:
                    throw new InvalidOperationException(
                        $"Hồ sơ với ID {documentId} đã được yêu cầu hoàn tiền cọc trước đó."
                    );
                case 2:
                    throw new InvalidOperationException(
                        $"Hồ sơ với ID {documentId} không đủ điều kiện để yêu cầu hoàn tiền: Đã chấp nhận hoàn."
                    );
            }
        }

        public async Task<string> UploadRefundProofAsync(IFormFile refundProof)
        {
            var refundProofUrl = await _uploadFileService.UploadAsync(refundProof);
            if (string.IsNullOrWhiteSpace(refundProofUrl))
            {
                throw new InvalidOperationException("Upload file thất bại, không có URL hợp lệ.");
            }

            return refundProofUrl;
        }
    }
}
