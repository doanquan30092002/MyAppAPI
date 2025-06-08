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

        public string BankAccount { get; set; }
        public string BankAccountNumber { get; set; }
        public string BankBranch { get; set; }

        public Guid CreateByTicket { get; set; }

        public DateTime CreateAtTicket { get; set; } = DateTime.Now;

        public DateTime UpdateAtTicket { get; set; } = DateTime.Now;

        public DateTime CreateAtDeposit { get; set; } = DateTime.Now;

        public bool StatusTicket { get; set; }

        public bool Status_deposit { get; set; }

        public bool StatusRefundDeposit { get; set; }
    }
}
