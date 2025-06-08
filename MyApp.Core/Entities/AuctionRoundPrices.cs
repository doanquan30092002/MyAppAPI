using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyApp.Core.Entities
{
    public class AuctionRoundPrices
    {
        [Key]
        public Guid AuctionRoundPriceId { get; set; }
        public Guid AuctionRoundId { get; set; }

        [ForeignKey("AuctionRoundId")]
        public AuctionRound AuctionRound { get; set; }

        public string UserName { get; set; }
        public string CitizenIdentification { get; set; }
        public string RecentLocation { get; set; }

        public string TagName { get; set; }

        public decimal AuctionPrice { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public Guid CreatedBy { get; set; }
    }
}
