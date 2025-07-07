using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyApp.Application.CQRS.Notifications.HasUnread.Command
{
    public class HasUnreadNotificationResponse
    {
        public bool HasUnread { get; set; }
    }
}
