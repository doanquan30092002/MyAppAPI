using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MyApp.Application.CQRS.Auction.WaitingPublic.Commands
{
    public interface IJobScheduler
    {
        string Schedule<T>(Expression<Func<T, Task>> methodCall, DateTimeOffset enqueueAt);
    }
}
