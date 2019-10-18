using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using System.Threading.Tasks;
using Common;
using Common.Entities;

namespace Flights.Store
{
    public interface IFlightStore
    {
        Task<ImmutableList<Flight>> Get();

        Task Add(Flight f);
    }
}
