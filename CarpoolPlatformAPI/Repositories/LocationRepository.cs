using CarpoolPlatformAPI.Data;
using CarpoolPlatformAPI.Models.Domain;
using CarpoolPlatformAPI.Repositories.IRepository;

namespace CarpoolPlatformAPI.Repositories
{
    public class LocationRepository : Repository<Location>, ILocationRepository
    {
        private readonly CarpoolPlatformDbContext _db;

        public LocationRepository(CarpoolPlatformDbContext db) : base(db)
        {
            _db = db;
        }
    }
}
