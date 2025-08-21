using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using MyApp.Application.CQRS.Auction.GetListAuction.Querries;

namespace MyApp.Application.CQRS.Auction.GetListAution.Querries
{
    public class GetListAuctionDocumentsRequest : IRequest<GetListAuctionDocumentsResponse>
    {
        public Guid AuctionId { get; set; }
        public string? CitizenIdentification { get; set; }
        public string? Name { get; set; }

        public string? TagName { get; set; }

        public int? StatusTicket { get; set; }
        public int? StatusDeposit { get; set; }

        public bool? IsAttended { get; set; }

        /*
        1: Đã yêu cầu hoàn tiền cọc
        2: Chấp nhận hoàn cọc
        3: Từ chối hoàn cọc
        */
        public int? StatusRefund { get; set; }

        public string? SortBy { get; set; }

        public bool IsAscending { get; set; } = true;

        public int? PageNumber { get; set; }

        public int? PageSize { get; set; }
    }
}
