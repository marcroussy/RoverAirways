using Common;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using System.Threading.Tasks;

namespace Flights.Store
{
    public class FlightStore
    {
        public FlightStore()
        {
        }

        public Task Add(Flight flight)
        {
            // Retrieve the storage account from the connection string.
            var account = Environment.GetEnvironmentVariable("AzureWebJobsStorage");
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(account);

            // Create the table client.
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

            // Create the CloudTable object that represents the "people" table.
            CloudTable table = tableClient.GetTableReference("RoverF");

            // Create a new customer entity.
            FlightStoreEntity customer1 = new FlightStoreEntity(flight);

            // Create the TableOperation object that inserts the customer entity.
            TableOperation insertOperation = TableOperation.Insert(customer1);

            // Execute the insert operation.
            return table.ExecuteAsync(insertOperation);
        }

        public async Task<ImmutableList<Flight>> Get()
        {

            // Retrieve the storage account from the connection string.
            var account = Environment.GetEnvironmentVariable("AzureWebJobsStorage");
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(account);

            // Create the table client.
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

            // Create the CloudTable object that represents the "people" table.
            CloudTable table = tableClient.GetTableReference("RoverF");

            // Construct the query operation for all customer entities where PartitionKey="Smith".
            TableQuery<FlightStoreEntity> query = new TableQuery<FlightStoreEntity>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "All"));

            var token = new TableContinuationToken();
            var entities = await table.ExecuteQuerySegmentedAsync(query, token);

            var list = new List<Flight>();
            foreach (var entity in entities)
            {
                list.Add(new Flight(entity.Id, entity.Departing, entity.Arriving, entity.Equipment));
            }
            return list.ToImmutableList();
        }
    }
}
