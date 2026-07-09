using CareGuide.Models.DTOs.Address;

namespace CareGuide.Core.Interfaces
{
    public interface IAddressService
    {
        Task<List<AddressDto>> GetAllAsync(int page, int pageSize, CancellationToken cancellationToken);
        Task<AddressDto> GetAsync(Guid id, CancellationToken cancellationToken);
        Task<AddressDto> CreateAsync(CreateAddressDto createAddressDto, CancellationToken cancellationToken);
        Task<AddressDto> UpdateAsync(Guid id, UpdateAddressDto updateAddressDto, CancellationToken cancellationToken);
        Task DeleteByIdsAsync(List<Guid> ids, CancellationToken cancellationToken);
    }
}
