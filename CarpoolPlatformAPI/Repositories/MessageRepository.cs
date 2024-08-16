using CarpoolPlatformAPI.Data;
using CarpoolPlatformAPI.Models.Domain;
using CarpoolPlatformAPI.Repositories.IRepository;

namespace CarpoolPlatformAPI.Repositories
{
    public class MessageRepository : Repository<Message>, IMessageRepository
    {
        private readonly CarpoolPlatformDbContext _db;

        public MessageRepository(CarpoolPlatformDbContext db) : base(db)
        {
            _db = db;
        }
    }
}
