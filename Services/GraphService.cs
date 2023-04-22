using Microsoft.Azure.Cosmos;
using Gremlin.Net.Driver;
using Gremlin.Net.Structure.IO.GraphSON;
using GremlinApi.Models;

namespace GremlinAPI.Services
{
    public class GraphService : IGraphService
    {
        private readonly IConfiguration _configuration;
        private readonly CosmosClient _cosmosClient;
        private readonly GremlinServer _gremlinServer;

        public GraphService(IConfiguration configuration)
        {
            _configuration = configuration;
            _cosmosClient = new CosmosClient(_configuration["CosmosDB:Endpoint"], _configuration["CosmosDB:Key"]);
            _gremlinServer = new GremlinServer(
                hostname: _configuration["CosmosDB:Endpoint"],
                port: 443,
                enableSsl: true,
                username: "/dbs/" + _configuration["CosmosDB:DatabaseId"] + "/colls/" + _configuration["CosmosDB:GraphId"],
                password: _configuration["CosmosDB:Key"]
            );
        }

        public async Task CreateDatabaseAndGraphIfNotExistsAsync()
        {
            await _cosmosClient.CreateDatabaseIfNotExistsAsync(_configuration["CosmosDB:DatabaseId"]);
            var database = _cosmosClient.GetDatabase(_configuration["CosmosDB:DatabaseId"]);
            await database.CreateContainerIfNotExistsAsync(new ContainerProperties
            {
                Id = _configuration["CosmosDB:GraphId"],
                PartitionKeyPath = _configuration["CosmosDB:PartitionKey"]
            });
        }

        public async Task AddPersonAsync(Person person)
        {
            var gremlinQuery = $"g.addV('person').property('id', '{person.Id}').property('name', '{person.Name}').property('age', {person.Age}).property('partitionKey','person')";
            await ExecuteGremlinQuery(gremlinQuery);
        }

        public async Task AddPlaceAsync(Place place)
        {
            var gremlinQuery = $"g.addV('place').property('id', '{place.Id}').property('name', '{place.Name}').property('city', '{place.City}').property('partitionKey','city')";
            await ExecuteGremlinQuery(gremlinQuery);
        }

        public async Task ConnectPersonToPlaceAsync(string personId, string placeId)
        {
            var gremlinQuery = $"g.V('{personId}').addE('livesAt').to(g.V('{placeId}'))";
            await ExecuteGremlinQuery(gremlinQuery);
        }

        private async Task ExecuteGremlinQuery(string gremlinQuery)
        {
            using var gremlinClient = new GremlinClient(_gremlinServer, new GraphSON2Reader(), new GraphSON2Writer(), "application/vnd.gremlin-v2.0+json");
            await gremlinClient.SubmitAsync<dynamic>(gremlinQuery);
        }
    }
}
