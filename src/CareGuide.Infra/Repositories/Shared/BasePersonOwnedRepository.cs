using CareGuide.Infra.Interfaces.Shared;
using CareGuide.Models.Entities.Shared;
using Microsoft.EntityFrameworkCore;

namespace CareGuide.Infra.Repositories.Shared
{
    public class BasePersonOwnedRepository<TEntity> : BaseRepository<TEntity>, IBasePersonOwnedRepository<TEntity> where TEntity : Entity, IPersonOwnedEntity
    {
        private readonly IQueryScope _queryScope;

        public BasePersonOwnedRepository(DbContext context, IQueryScope queryScope) : base(context)
        {
            _queryScope = queryScope;
        }

        protected IQueryable<TEntity> Scoped => _queryScope.Apply(context.Set<TEntity>());

        public override async Task<TEntity?> GetAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await Scoped.FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
        }

        public override async Task<List<TEntity>> GetManyAsync(IEnumerable<Guid> ids, int page, int pageSize, CancellationToken cancellationToken = default)
        {
            if (page < 1) throw new ArgumentException("Page must be greater than or equal to 1.", nameof(page));
            if (pageSize < 1) throw new ArgumentException("PageSize must be greater than or equal to 1.", nameof(pageSize));

            return await Scoped
                .Where(e => ids.Contains(e.Id))
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);
        }

        public override async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await Scoped.AnyAsync(e => e.Id == id, cancellationToken);
        }

        public override async Task<int> CountExistingAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default)
        {
            var idList = ids?.Distinct().ToList() ?? [];
            if (idList.Count == 0) return 0;

            return await Scoped
                .AsNoTracking()
                .CountAsync(e => idList.Contains(e.Id), cancellationToken);
        }

        public override async Task<TEntity?> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var entity = await Scoped.FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

            if (entity == null)
                return null;

            context.Remove(entity);
            await context.SaveChangesAsync(cancellationToken);
            return entity;
        }

        public override async Task<List<TEntity>> DeleteManyAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default)
        {
            var entities = await Scoped.Where(e => ids.Contains(e.Id)).ToListAsync(cancellationToken);

            if (entities.Count == 0)
                return new List<TEntity>();

            context.Set<TEntity>().RemoveRange(entities);
            await context.SaveChangesAsync(cancellationToken);
            return entities;
        }

        public async Task<List<TEntity>> GetAllByPersonAsync(int page, int pageSize, CancellationToken cancellationToken = default)
        {
            return await Scoped
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);
        }

        public async Task<TEntity?> GetFirstByPersonAsync(CancellationToken cancellationToken = default)
        {
            return await Scoped.FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<int> DeleteAllByPersonAsync(CancellationToken cancellationToken = default)
        {
            var entities = await Scoped.ToListAsync(cancellationToken);

            if (entities.Count == 0)
                return 0;

            context.Set<TEntity>().RemoveRange(entities);
            return await context.SaveChangesAsync(cancellationToken);
        }
    }
}
