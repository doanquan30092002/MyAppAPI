using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BuiltDB.Models
{
	public class Blogs
	{
		[Key]
		public Guid BlogId { get; set; }
		public string Title { get; set; }
		public string Content { get; set; }
		public string ThumbnailUrl { get; set; }
		public DateTime CreatedAt { get; set; } = DateTime.Now;
		public Guid CreatedBy { get; set; }

		[ForeignKey("CreatedBy")]
		public User CreatedByUser { get; set; }

		public DateTime UpdatedAt { get; set; } = DateTime.Now;
		public Guid UpdatedBy { get; set; }
		public bool Status { get; set; }

		public Guid BlogTypeId { get; set; }
		[ForeignKey("BlogTypeId")]
		public BlogType BlogType { get; set; }
	}
}
