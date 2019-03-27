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

namespace Flights
{
    [StorageAccount("AzureWebJobsStorage")]
    public static class Validator
    {
        private static FlightStore _store = new FlightStore();

        [return: Queue("validationscompleted")]
        [FunctionName("Validator")]
        public static async Task<ValidationsComplete> Run([TimerTrigger("0 */1 * * * *")]TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

            var flights = await _store.Get();

            if (flights.IsEmpty)
            {
                log.LogInformation($"No flights scheduled.");
            }

            foreach (var flight in flights)
            {
                // Perform some random validation for now
                log.LogInformation($"Flight {flight.Id} is valid.");
            }

            return new ValidationsComplete() {FlightIds = flights.Select(f => f.Id).ToList() , Succesful = true  } ;
        }

        public class ValidationsComplete
        {
            public bool Succesful { get; set; }
            public List<int> FlightIds { get; set; }
        }
    }
}
