using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace MyApp.Application.CQRS.Auction.CancelAuction.Commands
{
    public class CancelAuctionCommand
    {
        [Required(ErrorMessage = "Phiên đấu giá là bắt buộc.")]
        public Guid AuctionId { get; set; }

        [Required(ErrorMessage = "Văn bản hủy đấu giá là bắt buộc.")]
        public IFormFile CancelReasonFile { get; set; }

        [Required(ErrorMessage = "Lý do hủy phiên đấu giá là bắt buộc.")]
        public string CancelReason { get; set; }
    }
}
