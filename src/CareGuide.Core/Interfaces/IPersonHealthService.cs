using CareGuide.Models.DTOs.PersonHealth;
using CareGuide.Models.DTOs.Shared;

namespace CareGuide.Core.Interfaces
{
    public interface IPersonHealthService
    {
        Task<PagedResult<PersonHealthDto>> GetAllByPersonAsync(int page, int pageSize, CancellationToken cancellationToken);
        Task<PersonHealthDto> GetAsync(Guid id, CancellationToken cancellationToken);
        Task<PersonHealthDto> CreateAsync(CreatePersonHealthDto personHealth, CancellationToken cancellationToken);
        Task<PersonHealthDto> UpdateAsync(Guid id, UpdatePersonHealthDto personHealth, CancellationToken cancellationToken);
        Task DeleteAllByPersonAsync(CancellationToken cancellationToken);
        Task DeleteAsync(Guid id, CancellationToken cancellationToken);
        Task DeleteByIdsAsync(List<Guid> ids, CancellationToken cancellationToken);
    }
}
