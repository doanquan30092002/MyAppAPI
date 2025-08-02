using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amazon.Runtime.Internal;
using MediatR;

namespace MyApp.Application.CQRS.Auction.MarkAuctionAsSuccessful.Command
{
    public class MarkAuctionAsSuccessfulCommand : IRequest<bool>
    {
        [Required(ErrorMessage = "Phiên đấu giá là bắt buộc.")]
        public Guid AuctionId { get; set; }
    }
}
