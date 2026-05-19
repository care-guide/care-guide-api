using CareGuide.Infra.Interfaces.Shared;
using CareGuide.Models.Entities.Shared;
using Microsoft.EntityFrameworkCore;

namespace CareGuide.Infra.Repositories.Shared
{
    public class BaseRepository<TEntity> : IRepository<TEntity> where TEntity : Entity
    {
        protected readonly DbContext context;

        public BaseRepository(DbContext context)
        {
            this.context = context;
        }

        public virtual async Task<List<TEntity>> GetAllAsync(int page, int pageSize, CancellationToken cancellationToken = default)
        {
            if (page < 1) throw new ArgumentException("Page must be greater than or equal to 1.", nameof(page));
            if (pageSize < 1) throw new ArgumentException("PageSize must be greater than or equal to 1.", nameof(pageSize));

            return await context.Set<TEntity>()
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);
        }

        public virtual async Task<TEntity?> GetAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await context.Set<TEntity>().FindAsync([id], cancellationToken) ?? null;
        }

        public virtual async Task<List<TEntity>> GetManyAsync(IEnumerable<Guid> ids, int page, int pageSize, CancellationToken cancellationToken = default)
        {
            if (page < 1) throw new ArgumentException("Page must be greater than or equal to 1.", nameof(page));
            if (pageSize < 1) throw new ArgumentException("PageSize must be greater than or equal to 1.", nameof(pageSize));

            return await context.Set<TEntity>()
                .Where(e => ids.Contains(e.Id))
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);
        }

        public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await context.Set<TEntity>().AnyAsync(e => e.Id == id, cancellationToken);
        }

        public async Task<int> CountExistingAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default)
        {
            var idList = ids?.Distinct().ToList() ?? [];
            if (idList.Count == 0) return 0;

            return await context.Set<TEntity>()
                .AsNoTracking()
                .CountAsync(e => idList.Contains(e.Id), cancellationToken);
        }

        public async Task SaveChangesAsync(CancellationToken cancellationToken)
        {
            await context.SaveChangesAsync(cancellationToken);
        }

        public async Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            await context.Set<TEntity>().AddAsync(entity, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
            return entity;
        }

        public async Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            context.Entry(entity).State = EntityState.Modified;
            context.Set<TEntity>().Update(entity);
            await context.SaveChangesAsync(cancellationToken);
            return entity;
        }

        public async Task<TEntity?> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var entity = await context.Set<TEntity>().FindAsync([id], cancellationToken);

            if (entity == null)
                return null;

            context.Remove(entity);
            await context.SaveChangesAsync(cancellationToken);
            return entity;
        }

        public async Task<List<TEntity>> DeleteManyAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default)
        {
            var set = context.Set<TEntity>();
            var entities = await set.Where(e => ids.Contains(e.Id)).ToListAsync(cancellationToken);

            if (entities.Count == 0)
                return new List<TEntity>();

            set.RemoveRange(entities);
            await context.SaveChangesAsync(cancellationToken);
            return entities;
        }
    }
}
