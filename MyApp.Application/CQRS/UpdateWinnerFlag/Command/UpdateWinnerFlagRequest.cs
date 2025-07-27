using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using MyApp.Application.CQRS.UpdateStatusDeposit.Command;

namespace MyApp.Application.CQRS.UpdateFlagWinner.Command
{
    public class UpdateWinnerFlagRequest : IRequest<UpdateWinnerFlagResponse>
    {
        public Guid AuctionRoundPriceId { get; set; }
    }
}
