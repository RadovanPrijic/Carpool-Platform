using CarpoolPlatformAPI.Data;
using CarpoolPlatformAPI.Models.Domain;
using CarpoolPlatformAPI.Repositories.IRepository;
using Microsoft.EntityFrameworkCore;

namespace CarpoolPlatformAPI.Repositories
{
    public class LocationRepository : Repository<Location>, ILocationRepository
    {
        private readonly CarpoolPlatformDbContext _db;

        public LocationRepository(CarpoolPlatformDbContext db) : base(db)
        {
            _db = db;
        }

        public async Task<List<Location>> AddLocationsAsync(IEnumerable<Location> locations)
        {
            _db.Locations.AddRange(locations);
            await _db.SaveChangesAsync();
            return locations.ToList();
        }
    }
}
