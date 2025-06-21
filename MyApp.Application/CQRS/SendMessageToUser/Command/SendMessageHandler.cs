using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using MyApp.Application.Common.Services.SendMessage;

namespace MyApp.Application.CQRS.SendMessageToUser.Command
{
    public class SendMessageHandler : IRequestHandler<SendMessageCommand, bool>
    {
        private readonly IDictionary<SendMessageChannel, ISendMessage> _senders;

        public SendMessageHandler(IEnumerable<ISendMessage> senders)
        {
            _senders = senders.ToDictionary(x => x.Channel);
        }

        public async Task<bool> Handle(
            SendMessageCommand request,
            CancellationToken cancellationToken
        )
        {
            if (!_senders.TryGetValue(request.Channel, out var sender))
                throw new NotSupportedException("Kênh gửi tin nhắn không hỗ trợ.");

            string subject =
                request.Channel == SendMessageChannel.Email ? "Thông báo từ hệ thống" : "";

            return await sender.SendAsync(request.Contact, subject, request.Message);
        }
    }
}
