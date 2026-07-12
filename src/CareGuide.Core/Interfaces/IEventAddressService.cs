using CareGuide.Models.DTOs.EventAddress;
using CareGuide.Models.DTOs.Shared;

namespace CareGuide.Core.Interfaces
{
    public interface IEventAddressService
    {
        Task<PagedResult<EventAddressDto>> GetAllByEventIdAsync(int page, int pageSize, Guid eventId, CancellationToken cancellationToken);
        Task<EventAddressDto> AttachAsync(Guid eventId, Guid addressId, CancellationToken cancellationToken);
        Task DetachAsync(Guid eventId, Guid addressId, CancellationToken cancellationToken);
    }
}
