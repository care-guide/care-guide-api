using CareGuide.Models.Entities;

namespace CareGuide.Infra.Interfaces
{
    public interface IPersonPhoneRepository
    {
        Task AddAsync(PersonPhone entity, CancellationToken cancellationToken = default);
        Task<List<PersonPhone>> GetAllByPersonWithPhonesAsync(int page, int pageSize, CancellationToken cancellationToken = default);
        Task<int> CountByPersonAsync(CancellationToken cancellationToken = default);
        Task<List<PersonPhone>> GetAllByPersonUnpagedAsync(CancellationToken cancellationToken = default);
        Task<PersonPhone?> GetByPersonWithPhoneAsync(Guid phoneId, CancellationToken cancellationToken = default);
        Task<List<PersonPhone>> GetManyByPersonAndPhoneIdsAsync(IEnumerable<Guid> phoneIds, CancellationToken cancellationToken = default);
        Task DeleteAllByPersonAsync(CancellationToken cancellationToken = default);
        Task DeleteManyAsync(IEnumerable<Guid> phoneIds, CancellationToken cancellationToken = default);
    }
}
