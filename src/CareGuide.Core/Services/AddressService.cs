using AutoMapper;
using CareGuide.Core.Interfaces;
using CareGuide.Infra.Interfaces;
using CareGuide.Models.DTOs.Address;
using CareGuide.Models.Entities;

namespace CareGuide.Core.Services
{
    public class AddressService : IAddressService
    {
        private readonly IAddressRepository _addressRepository;
        private readonly IMapper _mapper;

        public AddressService(IAddressRepository addressRepository, IMapper mapper)
        {
            _addressRepository = addressRepository;
            _mapper = mapper;
        }

        public async Task<List<AddressDto>> GetAllAsync(int page, int pageSize, CancellationToken cancellationToken)
        {
            var list = await _addressRepository.GetAllAsync(page, pageSize, cancellationToken);
            return _mapper.Map<List<AddressDto>>(list);
        }

        public async Task<AddressDto> GetAsync(Guid id, CancellationToken cancellationToken)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("The id cannot be empty.", nameof(id));

            var entity = await _addressRepository.GetAsync(id, cancellationToken);

            if (entity == null)
                throw new KeyNotFoundException($"No address found with the ID {id}.");

            return _mapper.Map<AddressDto>(entity);
        }

        public async Task<AddressDto> CreateAsync(CreateAddressDto createAddressDto, CancellationToken cancellationToken)
        {
            if (createAddressDto == null)
                throw new ArgumentNullException(nameof(createAddressDto));

            var entity = _mapper.Map<Address>(createAddressDto);

            await _addressRepository.AddAsync(entity, cancellationToken);

            return _mapper.Map<AddressDto>(entity);
        }

        public async Task<AddressDto> UpdateAsync(Guid id, UpdateAddressDto updateAddressDto, CancellationToken cancellationToken)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("The id cannot be empty.", nameof(id));

            if (updateAddressDto == null)
                throw new ArgumentNullException(nameof(updateAddressDto));

            var existing = await _addressRepository.GetAsync(id, cancellationToken);

            if (existing == null)
                throw new KeyNotFoundException($"No address found with the ID {id}.");

            existing.PostalCode = updateAddressDto.PostalCode;
            existing.Number = updateAddressDto.Number;
            existing.Type = updateAddressDto.Type;
            existing.City = updateAddressDto.City;
            existing.Complement = updateAddressDto.Complement;
            existing.Country = updateAddressDto.Country;
            existing.District = updateAddressDto.District;
            existing.ReferencePoint = updateAddressDto.ReferencePoint;
            existing.State = updateAddressDto.State;

            var updated = await _addressRepository.UpdateAsync(existing, cancellationToken);
            return _mapper.Map<AddressDto>(updated);
        }

        public async Task DeleteByIdsAsync(List<Guid> ids, CancellationToken cancellationToken)
        {
            await _addressRepository.DeleteManyAsync(ids, cancellationToken);
        }
    }
}
