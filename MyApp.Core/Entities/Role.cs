using System.ComponentModel.DataAnnotations;

namespace BuiltDB.Models
{
	public class Role
	{
		[Key]
		public int RoleId { get; set; }
		public string RoleName { get; set; }
	}
}
