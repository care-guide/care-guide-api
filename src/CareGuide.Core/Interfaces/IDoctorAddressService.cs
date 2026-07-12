using CareGuide.Models.DTOs.DoctorAddress;
using CareGuide.Models.DTOs.Shared;

namespace CareGuide.Core.Interfaces
{
    public interface IDoctorAddressService
    {
        Task<PagedResult<DoctorAddressDto>> GetAllByDoctorAsync(int page, int pageSize, Guid doctorId, CancellationToken cancellationToken);
        Task<DoctorAddressDto> GetAsync(Guid addressId, Guid doctorId, CancellationToken cancellationToken);
        Task<DoctorAddressDto> CreateAsync(Guid doctorId, CreateDoctorAddressDto doctorAddressDto, CancellationToken cancellationToken);
        Task DeleteAllByDoctorAsync(Guid doctorId, CancellationToken cancellationToken);
        Task DeleteByIdsAsync(List<Guid> addressIds, Guid doctorId, CancellationToken cancellationToken);
    }
}
