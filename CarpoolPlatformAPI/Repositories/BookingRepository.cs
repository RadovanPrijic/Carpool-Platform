using CarpoolPlatformAPI.Data;
using CarpoolPlatformAPI.Models.Domain;
using CarpoolPlatformAPI.Repositories.IRepository;

namespace CarpoolPlatformAPI.Repositories
{
    public class BookingRepository : Repository<Booking>, IBookingRepository
    {
        private readonly CarpoolPlatformDbContext _db;

        public BookingRepository(CarpoolPlatformDbContext db) : base(db)
        {
            _db = db;
        }
    }
}