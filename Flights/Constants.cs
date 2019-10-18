using System;
using System.Collections.Generic;
using System.Text;

namespace Flights
{
    public static class HttpApiFunctions
    {
        public const string CreateFlight = "Scheduler";
        public const string GetFlights = "GetFlights";
    }

    public static class WorkerFunctions
    {
        public const string Validator = "Validator";
    }

    public static class ConfigurationSetting
    {
        public const string AzureWebJobsStorage = "AzureWebJobsStorage";
    }

    public static class HttpTriggerMethod
    {
        public const string Get = "get";
        public const string Post = "post";
    }

    public static class BindingParameter
    {
        public const string ScheduledFlightQueue = "flightscheduled";
        public const string ValidationCompletedQueue = "validationscompleted";
        public const string SchedulerBlobSchema = "schemas/SchedulerSchema.json";
        public const string ValidationTimer = "0 */1 * * * *";
    }

}
