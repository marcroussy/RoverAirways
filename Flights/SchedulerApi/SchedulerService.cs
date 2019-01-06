using Common;
using Common.Contracts;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Flights.SchedulerApi
{
    public class SchedulerService : IHttpTriggerService<Flight>
    {
        public Task<HttpTriggerResult> Process(Flight message)
        {
            return Task.FromResult(new HttpTriggerResult());
        }
    }
}
