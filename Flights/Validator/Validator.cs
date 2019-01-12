using System;
using System.Threading.Tasks;
using Flights.Store;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Flights
{

    public static class Validator
    {
        private static FlightStore _store = new FlightStore();

        [FunctionName("Validator")]
        public static async Task Run([TimerTrigger("0 */1 * * * *")]TimerInfo myTimer, ILogger log)
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

        }
    }
}
