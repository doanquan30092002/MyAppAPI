using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyApp.Core.Entities
{
    public class Auction
    {
        [Key]
        public Guid AuctionId { get; set; }
        public string AuctionName { get; set; }
        public string AuctionDescription { get; set; }
        public string AuctionRules { get; set; }
        public string? AuctionPlanningMap { get; set; }
        public DateTime RegisterOpenDate { get; set; }
        public DateTime RegisterEndDate { get; set; }

        public DateTime AuctionStartDate { get; set; }

        public DateTime AuctionEndDate { get; set; }

        public string? AuctionMap { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid CreatedBy { get; set; }

        [ForeignKey("CreatedBy")]
        public User CreatedByUser { get; set; }

        public DateTime UpdatedAt { get; set; }
        public Guid UpdatedBy { get; set; }
        public string QRLink { get; set; }

        public int NumberRoundMax { get; set; }

        //0: bản nháp, 1: công khai, 2:Hoàn thành, 3:Hủy, 4 : Chờ duyệt
        public int Status { get; set; }

        public string? WinnerData { get; set; }
        public int CategoryId { get; set; }

        [ForeignKey("CategoryId")]
        public AuctionCategory Category { get; set; }

        public bool? Updateable { get; set; }

        public string? CancelReasonFile { get; set; }

        public string? CancelReason { get; set; }

        public string? RejectReason { get; set; }
        public Guid? Auctioneer { get; set; }

        [ForeignKey("Auctioneer")]
        public User AuctioneerUser { get; set; }
    }
}
