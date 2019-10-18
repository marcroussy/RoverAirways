using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using Flights.Store;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Flights.Validator
{
    [StorageAccount(ConfigurationSetting.AzureWebJobsStorage)]
    public class ValidatorFunction
    {
        private readonly IFlightStore _store;

        public ValidatorFunction(IFlightStore store)
        {
            _store = store;
        }

        [return: Queue(BindingParameter.ValidationCompletedQueue)]
        [FunctionName(FunctionName.Validator)]
        public async Task<ValidationsComplete> Run([TimerTrigger(BindingParameter.ValidationTimer)]TimerInfo myTimer, ILogger log)
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
                    invalidFlights.Add(flight.FlightNo);
                }
                if (flight.Departing == flight.Arriving)
                {
                    invalidFlights.Add(flight.FlightNo);
                }

                if (invalidFlights.IndexOf(flight.FlightNo) == 0)
                {
                    validFlights.Add(flight.FlightNo);
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
