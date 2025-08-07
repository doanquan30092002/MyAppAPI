using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using MyApp.Application.Interfaces.IAuctionRepository;
using MyApp.Application.Interfaces.IExcelRepository;

namespace MyApp.Application.CQRS.AuctionDocuments.ExportExcelTransfer
{
    public class ExportExcelTransferHandler : IRequestHandler<ExportExcelTransferCommand, byte[]>
    {
        private readonly IExcelRepository _excelRepository;
        private readonly IAuctionRepository _auctionRepository;

        public ExportExcelTransferHandler(
            IExcelRepository excelRepository,
            IAuctionRepository auctionRepository
        )
        {
            _excelRepository = excelRepository;
            _auctionRepository = auctionRepository;
        }

        public async Task<byte[]> Handle(
            ExportExcelTransferCommand request,
            CancellationToken cancellationToken
        )
        {
            var auction = await _auctionRepository.FindAuctionByIdAsync(request.AuctionId);
            if (auction == null)
            {
                throw new KeyNotFoundException(
                    $"Phiên đấu giá với ID {request.AuctionId} không tồn tại."
                );
            }

            // status : 3 cancel, 2 completed
            if (auction.Status != 2 && auction.Status != 3)
            {
                throw new InvalidOperationException(
                    "Chỉ có thể xuất danh sách hoàn tiền cho phiên đấu giá đã bị hủy hoặc đã hoàn thành."
                );
            }

            var excelData = await _excelRepository.ExportRefundDocumentsExcelAsync(
                request.AuctionId
            );
            return excelData;
        }
    }
}
