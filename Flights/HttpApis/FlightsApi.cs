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
using Common.Entities;

namespace Flights.HttpApis
{
    public class FlightsApi
    {
        private readonly IFlightStore _store;

        public FlightsApi(IFlightStore store)
        {
            _store = store;
        }

        [FunctionName(HttpApiFunctions.GetFlights)]
        public async Task<IActionResult> GetFlights(
            [HttpTrigger(AuthorizationLevel.Function, HttpTriggerMethod.Get, Route = null)] HttpRequest req)
        {
            var list = await _store.Get();
            return new OkObjectResult(list);
        }

        [FunctionName(HttpApiFunctions.CreateFlight)]
        public async Task<IActionResult> CreateFlight(
            [HttpTrigger(AuthorizationLevel.Function, HttpTriggerMethod.Post, Route = null)] HttpRequest req,
            [Blob(BindingParameter.SchedulerBlobSchema, FileAccess.Read)] Stream validationSchema,
            [Queue(BindingParameter.ScheduledFlightQueue)]ICollector<Flight> queueCollector,
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

                return new OkObjectResult(flight);
            }
            catch (Exception ex)
            {
                log.LogError(ex.Message);

                return new InternalServerErrorObjectResult();
            }

        }
    }
}
