using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyApp.Core.Entities
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

        public DateTime? UpdatedAt { get; set; }
        public Guid? UpdatedBy { get; set; }

        //0: Draft
        //1: Pending
        //2: Publish
        //3: Disable
        //4: get 1, 2 and 3
        public int Status { get; set; }

        public string? Note { get; set; }
    }
}
