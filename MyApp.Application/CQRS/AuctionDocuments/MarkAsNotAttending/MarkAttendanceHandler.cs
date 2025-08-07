using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using MyApp.Application.Interfaces.IAuctionDocuments;

namespace MyApp.Application.CQRS.AuctionDocuments.MarkAsNotAttending
{
    public class MarkAttendanceHandler : IRequestHandler<MarkAttendanceRequest, bool>
    {
        private readonly IAuctionDocuments _auctionDocumentsRepository;

        public MarkAttendanceHandler(IAuctionDocuments auctionDocumentsRepository)
        {
            _auctionDocumentsRepository = auctionDocumentsRepository;
        }

        public async Task<bool> Handle(
            MarkAttendanceRequest request,
            CancellationToken cancellationToken
        )
        {
            var currentTime = DateTime.Now;

            foreach (var documentId in request.AuctionDocumentIds)
            {
                var document = await _auctionDocumentsRepository.GetDocumentByIdAsync(documentId);
                if (document == null)
                {
                    throw new KeyNotFoundException($"Hồ sơ với ID {documentId} không tồn tại.");
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

                if (currentTime < auction.AuctionStartDate)
                {
                    throw new InvalidOperationException(
                        $"Hồ sơ ID {documentId} không thể đánh điểm danh do trước ngày diễn ra đấu giá."
                    );
                }
            }

            return await _auctionDocumentsRepository.UpdateIsAttendedAsync(
                request.AuctionDocumentIds,
                request.IsAttended
            );
        }
    }
}
