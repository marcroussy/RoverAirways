using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Common;
using Flights.Store;
using Common.HttpHelpers;
using Newtonsoft.Json.Linq;

namespace Flights
{
    public static class Scheduler
    {
        private static FlightStore _store = new FlightStore();

        [FunctionName("Scheduler")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            try
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                if (!IsValid(requestBody))
                {
                    return new BadRequestObjectResult(requestBody);
                }

                dynamic data = JsonConvert.DeserializeObject(requestBody);

                var scheduled = DateTimeOffset.FromUnixTimeSeconds((long)data.scheduled);
                var revised = DateTimeOffset.FromUnixTimeSeconds((long)data.revised);

                var flight = new Flight(
                    (int)data.id,
                    (string)data.departing,
                    (string)data.arriving,
                    (string)data.equipment,
                    scheduled,
                    revised);

                await _store.Add(flight);

                return (ActionResult)new OkObjectResult(flight);
            }
            catch (Exception ex)
            {
                log.LogError(ex.Message);

                return (ActionResult)new InternalServerErrorObjectResult();
            }

        }

        private static bool IsValid(string requestBody)
        {
            JObject jo = JObject.Parse(requestBody);
            return (jo["scheduled"] != null ||
                    jo["revised"] != null ||
                    jo["id"] != null ||
                    jo["departing"] != null ||
                    jo["arriving"] != null ||
                    jo["equipment"] != null);
        }
    }
}
