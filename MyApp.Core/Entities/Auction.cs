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

        public bool Status { get; set; }

        public string? WinnerData { get; set; }
        public int CategoryId { get; set; }

        [ForeignKey("CategoryId")]
        public AuctionCategory Category { get; set; }

        public bool? Updateable { get; set; }
    }
}
