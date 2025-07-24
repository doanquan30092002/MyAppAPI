using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyApp.Application.CQRS.AuctionDocuments.FindHighestPriceAndFlag.Queries
{
    public class FindHighestPriceAndFlagRequest
    {
        [Required(ErrorMessage = "Phiên đấu giá là bắt buộc.")]
        public Guid AuctionId { get; set; }
    }
}
