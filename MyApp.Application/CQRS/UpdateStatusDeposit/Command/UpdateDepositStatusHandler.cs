using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using MyApp.Application.Common.Services.SendMessage;
using MyApp.Application.CQRS.GenarateNumbericalOrder;
using MyApp.Application.Interfaces.IGetListDocumentsRepository;
using MyApp.Application.Interfaces.IGetListRepository;
using MyApp.Application.Interfaces.IUpdateDepositStatus;
using MyApp.Core.Entities;

namespace MyApp.Application.CQRS.UpdateStatusDeposit.Command
{
    public class UpdateDepositStatusHandler
        : IRequestHandler<UpdateDepositStatusRequest, UpdateDepositStatusResponse>
    {
        private readonly IUpdateDepositStatus _updateDepositStatus;
        readonly IMediator _mediator;
        private readonly IEnumerable<ISendMessage> _sendMessages;

        public UpdateDepositStatusHandler(
            IUpdateDepositStatus updateDepositStatus,
            IEnumerable<ISendMessage> sendMessages,
            IMediator mediator
        )
        {
            _updateDepositStatus = updateDepositStatus;
            _sendMessages = sendMessages;
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

                int numberOrder = await _updateDepositStatus.GetOrderNumber(
                    request.AuctionDocumentsId
                );

                var emailList = await _updateDepositStatus.GetEmailList(request.AuctionDocumentsId);

                var emailSender = _sendMessages.FirstOrDefault(x =>
                    x.Channel == SendMessageChannel.Email
                );

                if (emailSender != null && emailList.Any())
                {
                    var subject =
                        "[Tuan Linh Digital Auction System] Thông báo đăng ký đấu giá thành công";
                    var content =
                        $"Bạn đã đăng ký đấu giá thành công tài sản với mã phiên đấu giá {request.AuctionId}. "
                        + $"Phiếu đăng ký của bạn có mã {request.AuctionDocumentsId} và số thứ tự là {numberOrder}.";
                    await emailSender.SendAsync("", subject, content, emailList);
                }
            }

            return updateDepositStatusResponse;
        }
    }
}
