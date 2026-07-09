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
