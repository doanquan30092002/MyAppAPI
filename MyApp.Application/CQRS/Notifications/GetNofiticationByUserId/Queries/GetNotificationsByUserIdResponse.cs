using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyApp.Application.CQRS.Notifications.GetNofiticationByUserId.Queries;
using MyApp.Core.Entities;

namespace MyApp.Application.CQRS.Notifications.GetNotificationByUserId.Queries
{
    public class GetNotificationsByUserIdResponse
    {
        public int TotalCount { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public List<NotificationDto> Notifications { get; set; }
    }
}
