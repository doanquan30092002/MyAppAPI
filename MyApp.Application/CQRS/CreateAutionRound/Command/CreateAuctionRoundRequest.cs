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

        public decimal? HighestBid { get; set; }

        public Guid CreatedBy { get; set; }
    }
}
