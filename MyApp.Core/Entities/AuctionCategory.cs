using System.ComponentModel.DataAnnotations;

namespace BuiltDB.Models
{
	public class AuctionCategory
	{
		[Key]
		public int CategoryId { get; set; }
		public string CategoryName { get; set; }
	}
}
