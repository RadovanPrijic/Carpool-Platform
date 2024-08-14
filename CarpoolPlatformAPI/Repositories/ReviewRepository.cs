using CarpoolPlatformAPI.Data;
using CarpoolPlatformAPI.Models.Domain;
using CarpoolPlatformAPI.Repositories.IRepository;

namespace CarpoolPlatformAPI.Repositories
{
    public class ReviewRepository : Repository<Review>, IReviewRepository
    {
        private readonly CarpoolPlatformDbContext _db;

        public ReviewRepository(CarpoolPlatformDbContext db) : base(db)
        {
            _db = db;
        }
    }
}
