using CareGuide.Models.DTOs.EventAddress;

namespace CareGuide.Core.Interfaces
{
    public interface IEventAddressService
    {
        Task<IReadOnlyCollection<EventAddressDto>> GetAllByEventIdAsync(Guid eventId, CancellationToken cancellationToken);
        Task<EventAddressDto> AttachAsync(Guid eventId, Guid addressId, CancellationToken cancellationToken);
        Task DetachAsync(Guid eventId, Guid addressId, CancellationToken cancellationToken);
    }
}
