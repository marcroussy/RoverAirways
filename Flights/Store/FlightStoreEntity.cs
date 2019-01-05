﻿using Common;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Text;

namespace Flights.Store
{
    public class FlightStoreEntity : TableEntity
    {
        public FlightStoreEntity()
        {
        }

        public FlightStoreEntity(Flight flight)
        {
            Id = flight.Id;
            Departing = flight.Departing;
            Arriving = flight.Arriving;
            Equipment = flight.Equipment;

            this.PartitionKey = "All";
            this.RowKey = Id.ToString();
        }

        public int Id { get; set; }
        public string Departing { get; set; }
        public string Arriving { get; set; }
        public string Equipment { get; set; }

    }
}