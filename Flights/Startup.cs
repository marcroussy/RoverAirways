using System;
using System.Collections.Generic;
using System.Text;
using Flights.Store;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(Flights.Startup))]

namespace Flights
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddSingleton<IFlightStore, FlightStore>();
        }
    }
}
