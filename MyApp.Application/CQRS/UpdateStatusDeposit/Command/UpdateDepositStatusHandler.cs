using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using MyApp.Application.CQRS.GenarateNumbericalOrder;
using MyApp.Application.Interfaces.IGetListDocumentsRepository;
using MyApp.Application.Interfaces.IGetListRepository;
using MyApp.Application.Interfaces.IUpdateDepositStatus;

namespace MyApp.Application.CQRS.UpdateStatusDeposit.Command
{
    public class UpdateDepositStatusHandler
        : IRequestHandler<UpdateDepositStatusRequest, UpdateDepositStatusResponse>
    {
        private readonly IUpdateDepositStatus _updateDepositStatus;
        readonly IMediator _mediator;

        public UpdateDepositStatusHandler(
            IUpdateDepositStatus updateDepositStatus,
            IMediator mediator
        )
        {
            _updateDepositStatus = updateDepositStatus;
            _mediator = mediator;
        }

        public async Task<UpdateDepositStatusResponse> Handle(
            UpdateDepositStatusRequest request,
            CancellationToken cancellationToken
        )
        {
            var updateDepositStatusResponse = await _updateDepositStatus.UpdateDepositStatus(
                request,
                cancellationToken
            );

            if (updateDepositStatusResponse.StatusUpdate)
            {
                var result = await _mediator.Send(
                    new GenarateNumbericalOrderRequest { AuctionId = request.AuctionId },
                    cancellationToken
                );
            }

            return updateDepositStatusResponse;
        }
    }
}
