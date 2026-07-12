using CareGuide.Models.DTOs.PersonPhone;
using CareGuide.Models.DTOs.Phone;
using CareGuide.Models.DTOs.Shared;

namespace CareGuide.Core.Interfaces
{
    public interface IPersonPhoneService
    {
        Task<PagedResult<PersonPhoneDto>> GetAllByPersonAsync(int page, int pageSize, CancellationToken cancellationToken);
        Task<PersonPhoneDto> GetAsync(Guid phoneId, CancellationToken cancellationToken);
        Task<PersonPhoneDto> CreateAsync(CreatePhoneDto personPhone, CancellationToken cancellationToken);
        Task<PersonPhoneDto> UpdateAsync(Guid id, UpdatePhoneDto personPhone, CancellationToken cancellationToken);
        Task DeleteAllByPersonAsync(CancellationToken cancellationToken);
        Task DeleteByIdsAsync(List<Guid> ids, CancellationToken cancellationToken);
    }
}
