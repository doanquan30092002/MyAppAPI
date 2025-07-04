using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using MyApp.Application.CQRS.Auction.GetListAuction.Querries;

namespace MyApp.Application.CQRS.Auction.GetListAution.Querries
{
    public class GetListAuctionRequest : IRequest<GetListAuctionResponse>
    {
        public Guid? AuctionId { get; set; }

        public string? AuctionName { get; set; }

        public int? CategoryId { get; set; }

        public int? Status { get; set; }

        public DateTime? RegisterOpenDate { get; set; }
        public DateTime? RegisterEndDate { get; set; }

        public DateTime? AuctionStartDate { get; set; }

        public DateTime? AuctionEndDate { get; set; }

        public string? SortBy { get; set; }

        public bool IsAscending { get; set; } = true;

        public int? PageNumber { get; set; }

        public int? PageSize { get; set; }
    }
}
