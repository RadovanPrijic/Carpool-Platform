using CarpoolPlatformAPI.Models.Domain;

namespace CarpoolPlatformAPI.Repositories.IRepository
{
    public interface ILocationRepository : IRepository<Location>
    {
        Task<List<Location>> AddLocationsAsync(IEnumerable<Location> locations);
    }
}
