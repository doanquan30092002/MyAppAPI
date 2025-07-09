using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace MyApp.Application.CQRS.ReceiveAuctionRegistrationForm
{
    public class ReceiveAuctionRegistrationFormRequest : IRequest<bool>
    {
        public Guid AuctionDocumentsId { get; set; }
    }
}
