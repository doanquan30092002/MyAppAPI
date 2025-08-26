using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyApp.Core.Entities;

namespace MyApp.Application.CQRS.GetListEnteredPrices.Querries
{
    public class GetListEnteredPricesResponse
    {
        public List<EnteredPriceDto> Items { get; set; } = new List<EnteredPriceDto>();
    }

    public class EnteredPriceDto
    {
        public Guid AuctionRoundPriceId { get; set; }
        public Guid AuctionRoundId { get; set; }

        public string UserName { get; set; }
        public string CitizenIdentification { get; set; }
        public string RecentLocation { get; set; }

        public string TagName { get; set; }

        public decimal AuctionPrice { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public Guid CreatedBy { get; set; }

        public bool FlagWinner { get; set; } = false;

        public decimal? StartingPrice { get; set; }
    }
}
