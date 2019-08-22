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
using Newtonsoft.Json.Schema;
using System.Collections.Generic;

namespace Flights
{
    public class Scheduler
    {
        private readonly IFlightStore _store;

        public Scheduler(IFlightStore store)
        {
            _store = store;
        }

        [FunctionName(FunctionName.Scheduler)]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            [Blob("schemas/SchedulerSchema.json", FileAccess.Read)] Stream validationSchema,
            [Queue("flightscheduled")]ICollector<Flight> queueCollector,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            try
            {
                string schemaJson = await new StreamReader(validationSchema).ReadToEndAsync();
                JSchema parsedSchema = JSchema.Parse(schemaJson);

                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var parsedRequest = JObject.Parse(requestBody);

                IList<string> errorMessages = new List<string>();
                bool validRequest = parsedRequest.IsValid(parsedSchema, out errorMessages);

                if (!validRequest)
                {
                    return new BadRequestObjectResult(errorMessages);
                }

                var flight = parsedRequest.ToObject<Flight>();

                await _store.Add(flight);

                queueCollector.Add(flight);

                return (ActionResult)new OkObjectResult(flight);
            }
            catch (Exception ex)
            {
                log.LogError(ex.Message);

                return (ActionResult)new InternalServerErrorObjectResult();
            }

        }
    }
}
