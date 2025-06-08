using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Data;

namespace BuiltDB.Models
{
	public class Account
	{
		[Key]
		public Guid AccountId { get; set; }
		public string PhoneNumber { get; set; }
		public string Email { get; set; }
		public string Password { get; set; }
		public DateTime CreatedAt { get; set; } = DateTime.Now;
		public Guid CreatedBy { get; set; }
		public DateTime UpdatedAt { get; set; } = DateTime.Now;
		public Guid UpdatedBy { get; set; }
		public bool IsActive { get; set; }

		public int RoleId { get; set; }
		[ForeignKey("RoleId")]
		public Role Role { get; set; }

		public Guid UserId { get; set; }
		[ForeignKey("UserId")]
		public User User { get; set; }
	}
}
