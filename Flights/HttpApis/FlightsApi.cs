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
using System.Linq;
using System.Net;

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

        [FunctionName(HttpApiFunctions.GetFlight)]
        public async Task<IActionResult> GetFlight(
            [HttpTrigger(AuthorizationLevel.Function, HttpTriggerMethod.Get, Route = null)] HttpRequest req)
        {
            var unsanitizedflightId = req.Query["flightId"];
            var validFlightId = int.TryParse(unsanitizedflightId, out var flightId);
            if (!validFlightId)
            {
                return ErrorResponse.BadRequest(type: "/invalid-flightid", instance: $"/flight/{unsanitizedflightId}");
            }

            var list = await _store.Get();
            var matched = list.FirstOrDefault(f => f.FlightNo == flightId);
            if (matched != null)
            {
                return new OkObjectResult(matched);
            }
            else
            {
                return ErrorResponse.NotFound(type: "/invalid-flightid", instance: $"/flight/{unsanitizedflightId}");
            }
        }

        [FunctionName(HttpApiFunctions.CreateFlight)]
        public async Task<IActionResult> CreateFlight(
            [HttpTrigger(AuthorizationLevel.Function, HttpTriggerMethod.Post, Route = null)] HttpRequest req,
            [Blob(BindingParameter.SchedulerBlobSchema, FileAccess.Read)] Stream validationSchema,
            [Queue(BindingParameter.ScheduledFlightQueue)]ICollector<Flight> queueCollector,
            ILogger log)
        {
            log.LogInformation("CreateFlight processing a request.");

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
                    return ErrorResponse.BadRequest(type: "/invalid-request", detail: errorMessages.Aggregate((i, j) => i + ", " + j));
                }

                var flight = parsedRequest.ToObject<Flight>();

                await _store.Add(flight);

                queueCollector.Add(flight);

                return new OkObjectResult(flight);
            }
            catch (Exception ex)
            {
                log.LogError(ex.Message);

                return ErrorResponse.InternalServerError(detail: ex.Message);
            }

        }
    }
}
