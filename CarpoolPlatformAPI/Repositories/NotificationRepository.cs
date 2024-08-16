using CarpoolPlatformAPI.Data;
using CarpoolPlatformAPI.Models.Domain;
using CarpoolPlatformAPI.Repositories.IRepository;

namespace CarpoolPlatformAPI.Repositories
{
    public class NotificationRepository : Repository<Notification>, INotificationRepository
    {
        private readonly CarpoolPlatformDbContext _db;

        public NotificationRepository(CarpoolPlatformDbContext db) : base(db)
        {
            _db = db;
        }
    }
}
