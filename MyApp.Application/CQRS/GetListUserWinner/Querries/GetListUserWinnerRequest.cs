using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace MyApp.Application.CQRS.GetListUserWinner.Querries
{
    public class GetListUserWinnerRequest : IRequest<GetListUserWinnerResponse>
    {
        public Guid AuctionId { get; set; }
    }
}
