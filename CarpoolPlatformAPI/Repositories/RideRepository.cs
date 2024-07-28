using CarpoolPlatformAPI.Data;
using CarpoolPlatformAPI.Models.Domain;
using CarpoolPlatformAPI.Repositories.IRepository;

namespace CarpoolPlatformAPI.Repositories
{
    public class RideRepository : Repository<Ride>, IRideRepository
    {
        private readonly CarpoolPlatformDbContext _db;

        public RideRepository(CarpoolPlatformDbContext db) : base(db)
        {
            _db = db;
        }

    }
}
