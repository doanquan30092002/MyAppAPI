using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BuiltDB.Models
{
	public class Blacklist
	{
		[Key]
		public Guid BlackListId { get; set; }

		public Guid UserId { get; set; }
		[ForeignKey("UserId")]
		public User User { get; set; }

		public string Reason { get; set; }

		public Guid AccountId { get; set; }
		public DateTime CreatedAt { get; set; } = DateTime.Now;
		public Guid CreatedBy { get; set; }
	}
}
