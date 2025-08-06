using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using MyApp.Application.Interfaces.IAuctionDocuments;

namespace MyApp.Application.CQRS.AuctionDocuments.ReviewRequestRefund.Command
{
    public class ReviewRequestRefundHandler : IRequestHandler<ReviewRequestRefundRequest, bool>
    {
        private readonly IAuctionDocuments _auctionDocumentsRepository;

        public ReviewRequestRefundHandler(IAuctionDocuments auctionDocumentsRepository)
        {
            _auctionDocumentsRepository = auctionDocumentsRepository;
        }

        public async Task<bool> Handle(
            ReviewRequestRefundRequest request,
            CancellationToken cancellationToken
        )
        {
            foreach (var documentId in request.AuctionDocumentIds)
            {
                var document = await _auctionDocumentsRepository.GetDocumentByIdAsync(documentId);
                if (document == null)
                {
                    throw new InvalidOperationException(
                        $"Hồ sơ với ID {documentId} không tồn tại."
                    );
                }

                if (request.StatusRefund == 2 && document.StatusDeposit != 1)
                {
                    throw new InvalidOperationException(
                        $"Không thể đồng ý yêu cầu hoàn tiền cho hồ sơ với ID {documentId} vì chưa nộp tiền cọc."
                    );
                }

                if (document.StatusRefund != 1)
                {
                    throw new InvalidOperationException(
                        $"Hồ sơ với ID {documentId} không ở trạng thái yêu cầu hoàn tiền cọc. Trạng thái hiện tại: {(document.StatusRefund == null ? "Chưa yêu cầu hoàn" : document.StatusRefund == 2 ? "Đã chấp nhận hoàn" : "Từ chối hoàn")}."
                    );
                }
            }

            var result = await _auctionDocumentsRepository.ReviewRequestRefundAsync(
                request.AuctionDocumentIds,
                request.StatusRefund,
                request.NoteReviewRefund
            );

            return result;
        }
    }
}
