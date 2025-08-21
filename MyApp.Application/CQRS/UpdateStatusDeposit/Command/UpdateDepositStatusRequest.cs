using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace MyApp.Application.CQRS.UpdateStatusDeposit.Command
{
    public class UpdateDepositStatusRequest : IRequest<UpdateDepositStatusResponse>
    {
        public Guid AuctionId { get; set; }
        public Guid AuctionDocumentsId { get; set; }
        public string? Note { get; set; }
    }
}
