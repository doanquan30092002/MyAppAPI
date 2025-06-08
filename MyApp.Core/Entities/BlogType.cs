using System.ComponentModel.DataAnnotations;

namespace MyApp.Core.Entities
{
    public class BlogType
    {
        [Key]
        public Guid BlogTypeId { get; set; }
        public string BlogsName { get; set; }
    }
}
