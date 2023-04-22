using GremlinApi.Models;

namespace GremlinAPI.Services
{
    public interface IGraphService
    {
        Task CreateDatabaseAndGraphIfNotExistsAsync();
        Task AddPersonAsync(Person person);
        Task AddPlaceAsync(Place place);
        Task ConnectPersonToPlaceAsync(string personId, string placeId);
    }
}
