using System.ComponentModel.DataAnnotations;

namespace MyApp.Core.Entities
{
    public class AuctionCategory
    {
        [Key]
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
    }
}
