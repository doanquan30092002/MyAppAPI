using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amazon.Runtime.Internal;
using MediatR;

namespace MyApp.Application.CQRS.PaymentDeposit.WebHookPaymentDeposit
{
    public class WebHookPaymentDepositCommand : IRequest<bool>
    {
        public Guid AuctionDocumentsId { get; set; }

        public bool Status { get; set; }

        public decimal Amount { get; set; }
    }
}
