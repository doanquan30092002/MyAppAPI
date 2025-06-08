using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BuiltDB.Models
{
	public class Notification
	{
		[Key]
		public Guid NotificationId { get; set; }
		public Guid UserId { get; set; }
		[ForeignKey("UserId")]
		public User User { get; set; }
		public string Message { get; set; }
		public int NotificationType { get; set; }
		public DateTime SentAt { get; set; } = DateTime.Now;
		public bool IsRead { get; set; }
		public DateTime UpdatedAt { get; set; } = DateTime.Now;
	}
}
