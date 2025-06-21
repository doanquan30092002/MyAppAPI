using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using MyApp.Application.CQRS.PaymentDeposit.RealTimeStatusDeposit;
using MyApp.Application.Interfaces.IPaymentDeposit;

namespace MyApp.Application.CQRS.PaymentDeposit.WebHookPaymentDeposit
{
    public class WebHookPaymentDepositHandler : IRequestHandler<WebHookPaymentDepositCommand, bool>
    {
        private readonly IPaymentDeposit _paymentDepositService;
        private readonly IHubContext<AuctionDepositHub> _hubContext;

        public WebHookPaymentDepositHandler(
            IPaymentDeposit paymentDepositService,
            IHubContext<AuctionDepositHub> hubContext
        )
        {
            _paymentDepositService = paymentDepositService;
            _hubContext = hubContext;
        }

        public async Task<bool> Handle(
            WebHookPaymentDepositCommand request,
            CancellationToken cancellationToken
        )
        {
            var result = await _paymentDepositService.UpdateStatusDepositAsync(
                request.AuctionDocumentsId,
                request.Status,
                request.Amount
            );

            if (result)
            {
                await _hubContext
                    .Clients.Group(request.AuctionDocumentsId.ToString())
                    .SendAsync(
                        "NotifyDepositStatusChanged",
                        new
                        {
                            AuctionDocumentsId = request.AuctionDocumentsId,
                            Status = request.Status,
                        }
                    );
            }

            return result;
        }
    }
}
