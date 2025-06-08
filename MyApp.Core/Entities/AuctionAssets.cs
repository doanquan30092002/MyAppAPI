using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BuiltDB.Models
{
	public class AuctionAssets
	{
		[Key]
		public Guid AuctionAssetsId { get; set; }
		public string TagName { get; set; }
		public decimal StartingPrice { get; set; }
		public string Unit { get; set; }
		public decimal Deposit { get; set; }
		public decimal RegistrationFee { get; set; }
		public string Description { get; set; }
		public DateTime CreatedAt { get; set; } = DateTime.Now;
		public Guid CreatedBy { get; set; }
		public DateTime UpdatedAt { get; set; } = DateTime.Now;
		public Guid UpdatedBy { get; set; }

		public Guid AuctionId { get; set; }
		[ForeignKey("AuctionId")]
		public Auction Auction { get; set; }
	}
}
