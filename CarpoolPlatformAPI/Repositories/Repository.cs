using CarpoolPlatformAPI.Repositories.IRepository;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Linq;
using CarpoolPlatformAPI.Data;

namespace CarpoolPlatformAPI.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly CarpoolPlatformDbContext _db;
        internal DbSet<T> dbSet;

        public Repository(CarpoolPlatformDbContext db)
        {
            _db = db;
            dbSet = _db.Set<T>();
        }

        public async Task<List<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null, string? includeProperties = null,
            int pageSize = 5, int pageNumber = 1)
        {
            IQueryable<T> query = dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (pageSize > 0)
            {
                if (pageSize > 25)
                {
                    pageSize = 25;
                }
                query = query.Skip(pageSize * (pageNumber - 1)).Take(pageSize);
            }

            if (includeProperties != null)
            {
                foreach (var includeProp in includeProperties.Split(new char[] { ',' }, 
                    StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
                {
                    query = query.Include(includeProp);
                }
            }

            return await query.ToListAsync();
        }

        public async Task<T?> GetAsync(Expression<Func<T, bool>> filter = null, string? includeProperties = null)
        {
            IQueryable<T> query = dbSet;

            query = query.Where(filter);

            if (includeProperties != null)
            {
                foreach (var includeProp in includeProperties.Split(new char[] { ',' },
                    StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
                {
                    query = query.Include(includeProp);
                }
            }

            return await query.FirstOrDefaultAsync();
        }

        public async Task<T> CreateAsync(T entity)
        {
            await dbSet.AddAsync(entity);
            await SaveAsync();
            return entity;
        }

        public async Task<T> UpdateAsync(T entity)
        {
            dbSet.Update(entity);
            await SaveAsync();
            return entity;
        }

        public async Task<T> RemoveAsync(T entity)
        {
            dbSet.Remove(entity);
            await SaveAsync();
            return entity;
        }

        public async Task SaveAsync()
        {
            await _db.SaveChangesAsync();
        }

    }
}
