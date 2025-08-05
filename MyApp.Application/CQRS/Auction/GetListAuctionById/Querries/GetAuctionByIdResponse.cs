using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyApp.Core.Entities;

namespace MyApp.Application.CQRS.Auction.GetListAuctionById.Querries
{
    public class GetAuctionByIdResponse
    {
        public string AuctionName { get; set; }
        public string AuctionDescription { get; set; }
        public string AuctionRules { get; set; }
        public string? AuctionPlanningMap { get; set; }
        public DateTime RegisterOpenDate { get; set; }
        public DateTime RegisterEndDate { get; set; }

        public DateTime AuctionStartDate { get; set; }

        public DateTime AuctionEndDate { get; set; }

        public string? AuctionMap { get; set; }
        public DateTime CreatedAt { get; set; }

        public String CreatedByUserName { get; set; }

        public DateTime UpdatedAt { get; set; }
        public String UpdatedByUserName { get; set; }
        public string QRLink { get; set; }

        public int NumberRoundMax { get; set; }

        public int Status { get; set; }

        public string? WinnerData { get; set; }
        public string CategoryName { get; set; }

        public string? CancelReasonFile { get; set; }

        public string? CancelReason { get; set; }

        public string? RejectReason { get; set; }

        public List<AuctionAssets> ListAuctionAssets { get; set; } = new List<AuctionAssets>();
    }
}
