using System.ComponentModel.DataAnnotations;

namespace BuiltDB.Models
{
	public class BlogType
	{
		[Key]
		public Guid BlogTypeId { get; set; }
		public string BlogsName { get; set; }
	}
}
