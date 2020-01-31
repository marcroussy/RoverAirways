using Common;
using Common.Entities;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flights.Store
{
    public class FlightStore : IFlightStore
    {
        private readonly string storageAccountName;
        private readonly string tableName;

        public FlightStore()
        {
            storageAccountName = Environment.GetEnvironmentVariable("AzureWebJobsStorage");
            tableName = "RoverF";
            Debug.WriteLine("Instantiated FlightStore");
        }

        public Task Add(Flight f)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(storageAccountName);

            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

            CloudTable table = tableClient.GetTableReference(tableName);

            FlightStoreEntity flight = new FlightStoreEntity(f);

            TableOperation insertOperation = TableOperation.Insert(flight);

            return table.ExecuteAsync(insertOperation);
        }

        public async Task<ImmutableList<Flight>> Get()
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(storageAccountName);

            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

            CloudTable table = tableClient.GetTableReference(tableName);

            TableQuery<FlightStoreEntity> query = new TableQuery<FlightStoreEntity>()
                .Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "All"));

            var token = new TableContinuationToken();

            var entities = await table.ExecuteQuerySegmentedAsync(query, token);

            return entities
                .Select(entity => new Flight(
                    entity.Id, 
                    entity.Departing, 
                    entity.Arriving, 
                    entity.Scheduled,
                    entity.Revised))
                .ToImmutableList();
        }
    }
}
