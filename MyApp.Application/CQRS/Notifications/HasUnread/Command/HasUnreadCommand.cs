using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amazon.Runtime.Internal;
using MediatR;

namespace MyApp.Application.CQRS.Notifications.HasUnread.Command
{
    public class HasUnreadCommand : IRequest<bool> { }
}
