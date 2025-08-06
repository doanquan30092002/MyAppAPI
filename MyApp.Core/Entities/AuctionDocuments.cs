using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyApp.Core.Entities
{
    public class AuctionDocuments
    {
        [Key]
        public Guid AuctionDocumentsId { get; set; }
        public Guid UserId { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }

        public Guid AuctionAssetId { get; set; }

        [ForeignKey("AuctionAssetId")]
        public AuctionAssets AuctionAsset { get; set; }

        public string? BankAccount { get; set; }
        public string? BankAccountNumber { get; set; }
        public string? BankBranch { get; set; }

        public Guid CreateByTicket { get; set; }

        public DateTime CreateAtTicket { get; set; } = DateTime.Now;

        public DateTime UpdateAtTicket { get; set; } = DateTime.Now;

        public DateTime? CreateAtDeposit { get; set; }

        /*
        0: chưa chuyển tiền phiếu đăng ký hồ sơ
        1: đã chuyển tiền phiếu đăng ký hồ sơ
        2: đã ký phiếu đăng ký hồ sơ
        3: da hoan tien ho so
        */
        public int StatusTicket { get; set; }

        /*
        0: chưa cọc
        1: đẵ cọc
        2: đã hoàn tiền cọc
        */
        public int StatusDeposit { get; set; }

        public int? NumericalOrder { get; set; }

        public string? Note { get; set; }

        public bool? IsAttended { get; set; }

        /*
        1: Đã yêu cầu hoàn tiền cọc
        2: Chấp nhận hoàn cọc
        3: Từ chối hoàn cọc
        */
        public int? StatusRefund { get; set; }
        public string? RefundReason { get; set; }
        public string? RefundProof { get; set; }
    }
}
