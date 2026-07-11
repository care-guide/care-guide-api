using CareGuide.Models.Entities.Shared;

namespace CareGuide.Infra.Interfaces.Shared
{
    public interface IBasePersonOwnedRepository<TEntity> : IRepository<TEntity> where TEntity : Entity, IPersonOwnedEntity
    {
        Task<List<TEntity>> GetAllByPersonAsync(int page, int pageSize, CancellationToken cancellationToken);
        Task<(List<TEntity> Items, int Total, int Page, int PageSize)> GetAllByPersonPagedAsync(int page, int pageSize, CancellationToken cancellationToken);
        Task<TEntity?> GetFirstByPersonAsync(CancellationToken cancellationToken);
        Task<int> DeleteAllByPersonAsync(CancellationToken cancellationToken);
    }
}
