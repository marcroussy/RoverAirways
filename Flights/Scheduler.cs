using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Common;
using Flights.Store;

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

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);

            var flight = new Flight((int)data.id, (string)data.departing, (string)data.arriving, (string)data.equipment);

            await _store.Add(flight);

            return
                (ActionResult)new OkObjectResult($"Flight scheduled");
        }
    }
}