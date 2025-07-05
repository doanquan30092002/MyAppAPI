using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amazon.Runtime.Internal;
using MediatR;

namespace MyApp.Application.CQRS.Notifications.MarkAsRead.Commands
{
    public class MarkAsReadRequest : IRequest<bool>
    {
        public Guid NotificationId { get; set; }

        public MarkAsReadRequest(Guid notificationId)
        {
            NotificationId = notificationId;
        }
    }
}
