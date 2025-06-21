using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using MyApp.Application.Interfaces.IPaymentDeposit;
using MyApp.Core.DTOs.AuctionDocumentsDTO;

namespace MyApp.Application.CQRS.PaymentDeposit.GetInforPaymentDeposit.Queries
{
    public class GetInforPaymentDepositHandler
        : IRequestHandler<GetInforPaymentDepositRequest, InforPaymentDepositResponse>
    {
        private readonly IPaymentDeposit _paymentDepositRepository;

        public GetInforPaymentDepositHandler(IPaymentDeposit paymentDepositRepository)
        {
            _paymentDepositRepository = paymentDepositRepository;
        }

        public async Task<InforPaymentDepositResponse> Handle(
            GetInforPaymentDepositRequest request,
            CancellationToken cancellationToken
        )
        {
            return await _paymentDepositRepository.GetPaymentDepositInfoAsync(
                request.AuctionDocumentsId
            );
        }
    }
}
