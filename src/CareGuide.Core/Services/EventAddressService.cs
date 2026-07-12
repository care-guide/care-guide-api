using AutoMapper;
using CareGuide.Core.Interfaces;
using CareGuide.Infra.Interfaces;
using CareGuide.Models.Constants;
using CareGuide.Models.DTOs.EventAddress;
using CareGuide.Models.DTOs.Shared;
using CareGuide.Models.Entities;

namespace CareGuide.Core.Services
{
    public class EventAddressService : IEventAddressService
    {
        private readonly IEventAddressRepository _eventAddressRepository;
        private readonly IEventRepository _eventRepository;
        private readonly IAddressRepository _addressRepository;
        private readonly IMapper _mapper;

        public EventAddressService(IEventAddressRepository eventAddressRepository, IEventRepository eventRepository, IAddressRepository addressRepository, IMapper mapper)
        {
            _eventAddressRepository = eventAddressRepository;
            _eventRepository = eventRepository;
            _addressRepository = addressRepository;
            _mapper = mapper;
        }

        public async Task<PagedResult<EventAddressDto>> GetAllByEventIdAsync(int page, int pageSize, Guid eventId, CancellationToken cancellationToken)
        {
            var (normalizedPage, normalizedPageSize) = PaginationNormalizer.Normalize(page, pageSize);

            var eventAddresses = await _eventAddressRepository.GetAllByEventAsync(normalizedPage, normalizedPageSize, eventId, cancellationToken);
            var total = await _eventAddressRepository.CountByEventAsync(eventId, cancellationToken);

            return new PagedResult<EventAddressDto>(_mapper.Map<List<EventAddressDto>>(eventAddresses), normalizedPage, normalizedPageSize, total);
        }

        public async Task<EventAddressDto> AttachAsync(Guid eventId, Guid addressId, CancellationToken cancellationToken)
        {
            var parentEvent = await _eventRepository.GetAsync(eventId, cancellationToken);

            if (parentEvent == null)
                throw new UnauthorizedAccessException("You are not authorized to access this event or it does not exist.");

            var address = await _addressRepository.GetAsync(addressId, cancellationToken);

            if (address == null)
                throw new KeyNotFoundException($"No address found with ID {addressId}.");

            var existing = await _eventAddressRepository.GetByEventAndAddressAsync(eventId, addressId, cancellationToken);

            if (existing != null)
                throw new InvalidOperationException($"The address with ID {addressId} is already attached to the event with ID {eventId}.");

            var entity = new EventAddress { EventId = eventId, AddressId = addressId };

            await _eventAddressRepository.AddAsync(entity, cancellationToken);

            var created = await _eventAddressRepository.GetByEventAndAddressAsync(eventId, addressId, cancellationToken);

            if (created == null)
                throw new InvalidOperationException("The created address attachment could not be retrieved.");

            return _mapper.Map<EventAddressDto>(created);
        }

        public async Task DetachAsync(Guid eventId, Guid addressId, CancellationToken cancellationToken)
        {
            var eventAddress = await _eventAddressRepository.GetByEventAndAddressAsync(eventId, addressId, cancellationToken);

            if (eventAddress == null)
                throw new KeyNotFoundException($"No address attachment found for event {eventId} and address {addressId}.");

            await _eventAddressRepository.DeleteAsync(eventAddress, cancellationToken);
        }
    }
}
