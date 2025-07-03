using System.Text;
using Microsoft.EntityFrameworkCore;
using MyApp.Application.Common.InformationBank;
using MyApp.Application.Common.Message;
using MyApp.Application.CQRS.RegisterAuctionDocument.Command;
using MyApp.Application.Interfaces.RegisterAuctionDocument.Repository;
using MyApp.Core.Entities;
using MyApp.Infrastructure.Data;

namespace MyApp.Infrastructure.Repositories.RegisterAuctionDocumentRepository
{
    public class RegisterAuctionDocumentRepository : IRegisterAuctionDocumentRepository
    {
        private readonly AppDbContext _context;

        public RegisterAuctionDocumentRepository(AppDbContext context)
        {
            this._context = context;
        }

        public Task<bool> CheckAuctionDocumentExsit(string? userId, string auctionAssetsId)
        {
            return _context.AuctionDocuments.AnyAsync(ad =>
                ad.UserId.ToString() == userId
                && ad.AuctionAssetId.ToString() == auctionAssetsId
                && ad.StatusTicket != 1
            );
        }

        public async Task<RegisterAuctionDocumentResponse> CreateQRForPayTicket(
            Guid auctionDocumentsId
        )
        {
            var auctionDocument = await _context
                .AuctionDocuments.Include(ad => ad.AuctionAsset)
                .ThenInclude(aa => aa.Auction)
                .FirstOrDefaultAsync(ad => ad.AuctionDocumentsId == auctionDocumentsId);
            string amount = auctionDocument.AuctionAsset.RegistrationFee.ToString("0");
            string content = $"Chuyen tien dang ky ho so DH{auctionDocumentsId}";
            string template = "compact";
            string download = "false";
            string accountNumber = InformationBank.ACCOUNT_NUMBER_TUAN_LINH;
            string beneficiaryBank = InformationBank.INFOR_BANK_BENEFICIARY_BANK;
            StringBuilder qrUrl = new StringBuilder();
            qrUrl.Append("https://qr.sepay.vn/img?");
            qrUrl.Append($"acc={Uri.EscapeDataString(accountNumber)}");
            qrUrl.Append($"&bank={Uri.EscapeDataString(beneficiaryBank)}");
            qrUrl.Append($"&amount={Uri.EscapeDataString(amount)}");
            qrUrl.Append($"&des={Uri.EscapeDataString(content)}");
            qrUrl.Append($"&template={Uri.EscapeDataString(template)}");
            qrUrl.Append($"&download={Uri.EscapeDataString(download)}");
            return new RegisterAuctionDocumentResponse
            {
                Code = 200,
                Message = Message.CREATE_QR_SUCCESS,
                QrUrl = qrUrl.ToString(),
                AuctionDocumentsId = auctionDocumentsId,
                AccountNumber = accountNumber,
                BeneficiaryBank = beneficiaryBank,
                AmountTicket = auctionDocument.AuctionAsset.RegistrationFee,
                Description = content,
            };
        }

        public async Task<Guid> InsertAuctionDocumentAsync(
            string auctionAssetsId,
            string? userId,
            string? bankAccount,
            string? bankAccountNumber,
            string? bankBranch
        )
        {
            var auctionDocument = new AuctionDocuments
            {
                AuctionDocumentsId = Guid.NewGuid(),
                UserId = Guid.Parse(userId),
                AuctionAssetId = Guid.Parse(auctionAssetsId),
                BankAccount = !string.IsNullOrWhiteSpace(bankAccount) ? bankAccount.Trim() : null,
                BankAccountNumber = !string.IsNullOrWhiteSpace(bankAccountNumber)
                    ? bankAccountNumber.Trim()
                    : null,
                BankBranch = !string.IsNullOrWhiteSpace(bankBranch) ? bankBranch.Trim() : null,
                CreateByTicket = Guid.Parse(userId),
                CreateAtTicket = DateTime.Now,
                UpdateAtTicket = DateTime.Now,
                CreateAtDeposit = null,
                StatusTicket = 0, // 0: chưa chuyển tiền phiếu đăng ký hồ sơ
                StatusDeposit = 0,
                NumericalOrder = null,
            };
            try
            {
                await _context.AuctionDocuments.AddAsync(auctionDocument);
                await _context.SaveChangesAsync();
                return auctionDocument.AuctionDocumentsId;
            }
            catch (Exception)
            {
                return Guid.Empty; // Handle the exception as needed
            }
        }

        public async Task<bool> UpdateStatusTicketAndGetUserIdAsync(Guid auctionDocumentsId)
        {
            var auctionDocument = await _context.AuctionDocuments.FindAsync(auctionDocumentsId);
            if (auctionDocument == null)
            {
                return false; // Auction document not found
            }
            try
            {
                auctionDocument.StatusTicket = 1; // 1: đã chuyển tiền phiếu đăng ký hồ sơ
                auctionDocument.UpdateAtTicket = DateTime.Now;
                _context.AuctionDocuments.Update(auctionDocument);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false; // Handle the exception as needed
            }
        }
    }
}
