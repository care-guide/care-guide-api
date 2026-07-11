using CareGuide.Models.Entities.Shared;

namespace CareGuide.Infra.Interfaces.Shared
{
    public interface IRepository<T> where T : Entity
    {
        Task<List<T>> GetAllAsync(int page, int pageSize, CancellationToken cancellationToken);
        Task<(List<T> Items, int Total, int Page, int PageSize)> GetAllPagedAsync(int page, int pageSize, CancellationToken cancellationToken);
        Task<T?> GetAsync(Guid id, CancellationToken cancellationToken);
        Task<List<T>> GetManyAsync(IEnumerable<Guid> ids, int page, int pageSize, CancellationToken cancellationToken);
        Task<T> AddAsync(T entity, CancellationToken cancellationToken);
        Task SaveChangesAsync(CancellationToken cancellationToken);
        Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken);
        Task<int> CountExistingAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken);
        Task<T> UpdateAsync(T entity, CancellationToken cancellationToken);
        Task<T?> DeleteAsync(Guid id, CancellationToken cancellationToken);
        Task<List<T>> DeleteManyAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken);
    }
}
