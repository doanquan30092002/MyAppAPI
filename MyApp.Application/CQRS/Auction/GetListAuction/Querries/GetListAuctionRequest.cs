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

        // 1: Danh sách phiên đấu giá đang thu hồ sơ
        // 2: Danh sách phiên đấu giá chuẩn bị tổ chức
        // 3: Danh sách phiên đấu giá hôm nay tổ chức
        // 4: Danh sách phiên đấu giá đã kết thúc
        public int? ConditionAuction { get; set; }

        public string? SortBy { get; set; }

        public bool IsAscending { get; set; } = true;

        public int? PageNumber { get; set; }

        public int? PageSize { get; set; }
    }
}
