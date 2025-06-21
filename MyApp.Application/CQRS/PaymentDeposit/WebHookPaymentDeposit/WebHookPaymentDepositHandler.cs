using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using MyApp.Application.Interfaces.IPaymentDeposit;

namespace MyApp.Application.CQRS.PaymentDeposit.WebHookPaymentDeposit
{
    public class WebHookPaymentDepositHandler : IRequestHandler<WebHookPaymentDepositCommand, bool>
    {
        private readonly IPaymentDeposit _paymentDepositService;

        public WebHookPaymentDepositHandler(IPaymentDeposit paymentDepositService)
        {
            _paymentDepositService = paymentDepositService;
        }

        public async Task<bool> Handle(
            WebHookPaymentDepositCommand request,
            CancellationToken cancellationToken
        )
        {
            return await _paymentDepositService.UpdateStatusDepositAsync(
                request.AuctionDocumentsId,
                request.Status,
                request.Amount
            );
        }
    }
}
