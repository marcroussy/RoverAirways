using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using Flights.Services;
using Flights.Store;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Flights
{
    [StorageAccount(ConfigurationSetting.AzureWebJobsStorage)]
    public class Validator
    {
        private readonly IFlightStore _store;

        public Validator(IFlightStore store)
        {
            _store = store;
        }

        [return: Queue("validationscompleted")]
        [FunctionName(FunctionName.Validator)]
        public async Task<ValidationsComplete> Run([TimerTrigger("0 */1 * * * *")]TimerInfo myTimer, ILogger log)
        {
            var flights = await _store.Get();

            if (flights.IsEmpty)
            {
                log.LogInformation($"No flights to validate.");
                return null;
            }

            var validFlights = new List<int>();
            var invalidFlights = new List<int>();

            foreach (var flight in flights)
            {
                if (flight.Revised <= flight.Scheduled)
                {
                    invalidFlights.Add(flight.Id);
                }
                if (flight.Departing == flight.Arriving)
                {
                    invalidFlights.Add(flight.Id);
                }

                if (invalidFlights.IndexOf(flight.Id) == 0)
                {
                    validFlights.Add(flight.Id);
                }
            }

            return new ValidationsComplete() { ValidFlightIds = validFlights, InvalidFlightIds = invalidFlights };
        }



        public class ValidationsComplete
        {
            public IEnumerable<int> ValidFlightIds { get; set; }
            public IEnumerable<int> InvalidFlightIds { get; set; }
        }
    }
}
