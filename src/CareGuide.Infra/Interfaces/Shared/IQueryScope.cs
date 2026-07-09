using CareGuide.Models.Entities.Shared;

namespace CareGuide.Infra.Interfaces.Shared
{
    public interface IQueryScope
    {
        IQueryable<TEntity> Apply<TEntity>(IQueryable<TEntity> query) where TEntity : IPersonOwnedEntity;
    }
}
