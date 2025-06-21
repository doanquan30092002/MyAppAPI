using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MyApp.Application.Interfaces.IPaymentDeposit;
using MyApp.Core.DTOs.AuctionDocumentsDTO;
using MyApp.Infrastructure.Data;

namespace MyApp.Infrastructure.Repositories.PaymentDepositRepository
{
    public class PaymentDepositRepository : IPaymentDeposit
    {
        private readonly AppDbContext _dbContext;

        public PaymentDepositRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<InforPaymentDepositResponse> GetPaymentDepositInfoAsync(
            Guid auctionDocumentId
        )
        {
            var auctionDocument = await _dbContext
                .AuctionDocuments.Include(ad => ad.AuctionAsset)
                .ThenInclude(aa => aa.Auction)
                .FirstOrDefaultAsync(ad => ad.AuctionDocumentsId == auctionDocumentId);

            if (auctionDocument == null || auctionDocument.AuctionAsset == null)
                throw new KeyNotFoundException(
                    "Không tìm thấy hồ sơ đấu giá hoặc tài sản đấu giá tương ứng."
                );

            if (auctionDocument.StatusDeposit)
                throw new InvalidOperationException("Hồ sơ này đã nộp cọc, không thể nộp lại.");

            var auction = auctionDocument.AuctionAsset.Auction;
            if (auction == null)
                throw new KeyNotFoundException("Không tìm thấy phiên đấu giá.");

            var now = DateTime.Now;
            var hanNopCoc = auction.AuctionStartDate.AddDays(-3);
            if (now > hanNopCoc)
                throw new InvalidOperationException(
                    "Đã quá hạn nộp cọc (chỉ được nộp trước 3 ngày so với ngày bắt đầu đấu giá)."
                );

            var depositAmount = auctionDocument.AuctionAsset.Deposit;

            var bankInfo = new InforBankTuanLinh
            {
                AccountNumber = "24059992699",
                BeneficiaryBank = "Agribank",
            };

            string soTaiKhoan = bankInfo.AccountNumber;
            string nganHang = "Agribank";
            string soTien = depositAmount.ToString("0");
            string noiDung = $"Chuyen tien dat coc DH{auctionDocumentId}";
            string template = "compact";
            string download = "false";

            var query =
                $"acc={Uri.EscapeDataString(soTaiKhoan)}"
                + $"&bank={Uri.EscapeDataString(nganHang)}"
                + $"&amount={Uri.EscapeDataString(soTien)}"
                + $"&des={Uri.EscapeDataString(noiDung)}"
                + $"&template={Uri.EscapeDataString(template)}"
                + $"&download={Uri.EscapeDataString(download)}";

            var qrUrl = $"https://qr.sepay.vn/img?{query}";

            return new InforPaymentDepositResponse { QrUrl = qrUrl, InforBankTuanLinh = bankInfo };
        }

        public async Task<bool> UpdateStatusDepositAsync(
            Guid auctionDocumentId,
            bool statusDeposit,
            decimal amount
        )
        {
            var auctionDocument = await _dbContext
                .AuctionDocuments.Include(ad => ad.AuctionAsset)
                .FirstOrDefaultAsync(ad => ad.AuctionDocumentsId == auctionDocumentId);

            if (auctionDocument == null)
                throw new KeyNotFoundException("Không tìm thấy hồ sơ đấu giá.");

            if (auctionDocument.AuctionAsset == null)
                throw new KeyNotFoundException("Không tìm thấy tài sản đấu giá.");

            if (auctionDocument.AuctionAsset.Deposit != amount)
                throw new InvalidOperationException(
                    "Số tiền đặt cọc đã bị thay đổi, xin vui lòng liên hệ để khắc phục sớm nhất."
                );

            auctionDocument.StatusDeposit = statusDeposit;
            auctionDocument.UpdateAtTicket = DateTime.Now;

            _dbContext.AuctionDocuments.Update(auctionDocument);
            await _dbContext.SaveChangesAsync();
            return true;
        }
    }
}
