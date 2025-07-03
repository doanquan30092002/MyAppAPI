using System.ComponentModel.DataAnnotations.Schema;
using MyApp.Core.Entities;

namespace MyApp.Application.CQRS.DetailAuctionDocument.Queries
{
    public class DetailAuctionDocumentResponse
    {
        public Guid AuctionDocumentsId { get; set; }
        public Guid UserId { get; set; }

        public Guid AuctionAssetId { get; set; }

        public string BankAccount { get; set; }
        public string BankAccountNumber { get; set; }
        public string BankBranch { get; set; }

        public Guid CreateByTicket { get; set; }

        public DateTime CreateAtTicket { get; set; }

        public DateTime UpdateAtTicket { get; set; }

        public DateTime? CreateAtDeposit { get; set; }

        /*
        0: chưa chuyển tiền phiếu đăng ký hồ sơ
        1: đã chuyển tiền phiếu đăng ký hồ sơ
        2: đã ký phiếu đăng ký hồ sơ
        */
        public int StatusTicket { get; set; }

        public bool StatusDeposit { get; set; }

        public bool StatusRefundDeposit { get; set; }

        public int? NumericalOrder { get; set; }
        public string? Note { get; set; }
    }
}
