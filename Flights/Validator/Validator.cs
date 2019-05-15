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
    [StorageAccount("AzureWebJobsStorage")]
    public class Validator
    {
        private readonly IFlightStore _store;
        private readonly IWarningGenerator _warnings;
        public Validator(IFlightStore store, IWarningGenerator warnings)
        {
            _store = store;
            _warnings = warnings;
        }

        [return: Queue("validationscompleted")]
        [FunctionName("Validator")]
        public async Task<ValidationsComplete> Run([TimerTrigger("0 */1 * * * *")]TimerInfo myTimer, ILogger log)
        {
            var flights = await _store.Get();

            if (flights.IsEmpty)
            {
                log.LogInformation($"No flights scheduled.");
            }

            foreach (var flight in flights)
            {
                if (flight.Revised <= flight.Scheduled)
                {
                    _warnings.Send();        
                }
                log.LogInformation($"Flight {flight.Id} is valid.");
            }

            return new ValidationsComplete() {FlightIds = flights.Select(f => f.Id).ToList() , Succesful = true  } ;
        }



        public class ValidationsComplete
        {
            public bool Succesful { get; set; }
            public IEnumerable<int> FlightIds { get; set; }
        }
    }
}
