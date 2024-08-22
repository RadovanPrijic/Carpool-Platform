using CarpoolPlatformAPI.Data;
using CarpoolPlatformAPI.Models.Domain;
using CarpoolPlatformAPI.Repositories.IRepository;

namespace CarpoolPlatformAPI.Repositories
{
    public class PictureRepository : Repository<Picture>, IPictureRepository
    {
        private readonly CarpoolPlatformDbContext _db;

        public PictureRepository(CarpoolPlatformDbContext db) : base(db)
        {
            _db = db;
        }
    }
}
