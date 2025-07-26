using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using MyApp.Core.DTOs.AuctionAssetsDTO;

namespace MyApp.Application.CQRS.AuctionAssetsV2.GetAuctionAssetsHighestBid.Queries
{
    public class GetAssetsHighestBidRequest : IRequest<List<AuctionAssetsWithHighestBidResponse>>
    {
        public Guid AuctionId { get; set; }
    }
}
