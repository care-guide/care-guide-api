using CareGuide.Models.DTOs.DoctorSpecialty;
using CareGuide.Models.DTOs.Shared;

namespace CareGuide.Core.Interfaces
{
    public interface IDoctorSpecialtyService
    {
        Task<PagedResult<DoctorSpecialtyDto>> GetAllByDoctorAsync(int page, int pageSize, Guid doctorId, CancellationToken cancellationToken);
        Task<DoctorSpecialtyDto> GetAsync(Guid id, Guid doctorId, CancellationToken cancellationToken);
        Task<DoctorSpecialtyDto> CreateAsync(Guid doctorId, CreateDoctorSpecialtyDto doctorSpecialtyDto, CancellationToken cancellationToken);
        Task<DoctorSpecialtyDto> UpdateAsync(Guid id, Guid doctorId, UpdateDoctorSpecialtyDto doctorSpecialtyDto, CancellationToken cancellationToken);
        Task DeleteAllByDoctorAsync(Guid doctorId, CancellationToken cancellationToken);
        Task DeleteByIdsAsync(List<Guid> ids, Guid doctorId, CancellationToken cancellationToken);
    }
}
