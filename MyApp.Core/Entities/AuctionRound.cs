using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyApp.Core.Entities
{
    public class AuctionRound
    {
        [Key]
        public Guid AuctionRoundId { get; set; }
        public Guid AuctionId { get; set; }

        [ForeignKey("AuctionId")]
        public Auction Auction { get; set; }

        public int RoundNumber { get; set; }

        // 1 : đang diễn ra
        // 2 : kết thúc
        public int Status { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public Guid CreatedBy { get; set; }
    }
}
