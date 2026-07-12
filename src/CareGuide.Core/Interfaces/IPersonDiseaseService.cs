using CareGuide.Models.DTOs.PersonDisease;
using CareGuide.Models.DTOs.Shared;

namespace CareGuide.Core.Interfaces
{
    public interface IPersonDiseaseService
    {
        Task<PagedResult<PersonDiseaseDto>> GetAllByPersonAsync(int page, int pageSize, CancellationToken cancellationToken);
        Task<PersonDiseaseDto> GetAsync(Guid id, CancellationToken cancellationToken);
        Task<PersonDiseaseDto> CreateAsync(CreatePersonDiseaseDto personDisease, CancellationToken cancellationToken);
        Task<PersonDiseaseDto> UpdateAsync(Guid id, UpdatePersonDiseaseDto personDisease, CancellationToken cancellationToken);
        Task DeleteAllByPersonAsync(CancellationToken cancellationToken);
        Task DeleteByIdsAsync(List<Guid> ids, CancellationToken cancellationToken);
    }
}
