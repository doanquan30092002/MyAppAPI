using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Hangfire;

namespace MyApp.Application.CQRS.Auction.WaitingPublic.Commands
{
    public class HangfireJobScheduler : IJobScheduler
    {
        private readonly IBackgroundJobClient _client;

        public HangfireJobScheduler(IBackgroundJobClient client)
        {
            _client = client;
        }

        public string Schedule<T>(Expression<Func<T, Task>> methodCall, DateTimeOffset enqueueAt)
        {
            return _client.Schedule(methodCall, enqueueAt);
        }
    }
}
