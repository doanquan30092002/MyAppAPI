using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amazon.Runtime.Internal;
using MediatR;
using MyApp.Application.CQRS.Notifications.GetNofiticationByUserId.Queries;

namespace MyApp.Application.CQRS.Notifications.GetNotificationById.Queries
{
    public class GetNotificationsByIdRequest : IRequest<NotificationDto>
    {
        public Guid NotificationId { get; set; }
    }
}
