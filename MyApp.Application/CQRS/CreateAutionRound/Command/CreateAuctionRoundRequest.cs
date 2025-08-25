using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using MyApp.Application.CQRS.SignUp.SignUpUser.Command;

namespace MyApp.Application.CQRS.CreateAutionRound.Command
{
    public class CreateAuctionRoundRequest : IRequest<CreateAuctionRoundResponse>
    {
        public Guid AuctionId { get; set; }

        public Guid CreatedBy { get; set; }

        public decimal? PriceMin { get; set; }
        public decimal? PriceMax { get; set; }
        public decimal? TotalPriceMax { get; set; }
    }
}
