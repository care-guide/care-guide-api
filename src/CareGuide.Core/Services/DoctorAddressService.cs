using AutoMapper;
using CareGuide.Core.Interfaces;
using CareGuide.Infra.Interfaces;
using CareGuide.Models.Constants;
using CareGuide.Models.DTOs.DoctorAddress;
using CareGuide.Models.DTOs.Shared;
using CareGuide.Models.Entities;

namespace CareGuide.Core.Services
{
    public class DoctorAddressService : IDoctorAddressService
    {
        private readonly IDoctorAddressRepository _doctorAddressRepository;
        private readonly IAddressRepository _addressRepository;
        private readonly IMapper _mapper;

        public DoctorAddressService(IDoctorAddressRepository doctorAddressRepository, IAddressRepository addressRepository, IMapper mapper)
        {
            _doctorAddressRepository = doctorAddressRepository;
            _addressRepository = addressRepository;
            _mapper = mapper;
        }

        public async Task<PagedResult<DoctorAddressDto>> GetAllByDoctorAsync(int page, int pageSize, Guid doctorId, CancellationToken cancellationToken)
        {
            var (normalizedPage, normalizedPageSize) = PaginationNormalizer.Normalize(page, pageSize);

            var doctorAddresses = await _doctorAddressRepository.GetAllByDoctorWithAddressesAsync(normalizedPage, normalizedPageSize, doctorId, cancellationToken);
            var total = await _doctorAddressRepository.CountByDoctorAsync(doctorId, cancellationToken);

            if (total == 0)
                throw new KeyNotFoundException($"No address records found for the doctor with ID {doctorId}.");

            return new PagedResult<DoctorAddressDto>(_mapper.Map<List<DoctorAddressDto>>(doctorAddresses), normalizedPage, normalizedPageSize, total);
        }

        public async Task<DoctorAddressDto> GetAsync(Guid addressId, Guid doctorId, CancellationToken cancellationToken)
        {
            var doctorAddress = await _doctorAddressRepository.GetByDoctorWithAddressAsync(addressId, doctorId, cancellationToken);

            if (doctorAddress == null)
                throw new UnauthorizedAccessException("You are not authorized to access this address record or it does not exist.");

            return _mapper.Map<DoctorAddressDto>(doctorAddress);
        }

        public async Task<DoctorAddressDto> CreateAsync(Guid doctorId, CreateDoctorAddressDto doctorAddressDto, CancellationToken cancellationToken)
        {
            if (doctorAddressDto == null)
                throw new ArgumentNullException(nameof(doctorAddressDto));

            var addressExists = await _addressRepository.ExistsAsync(doctorAddressDto.AddressId, cancellationToken);

            if (!addressExists)
                throw new KeyNotFoundException($"No address found with the ID {doctorAddressDto.AddressId}.");

            var doctorAddress = new DoctorAddress
            {
                DoctorId = doctorId,
                AddressId = doctorAddressDto.AddressId
            };

            await _doctorAddressRepository.AddAsync(doctorAddress, cancellationToken);

            var createdDoctorAddress = await _doctorAddressRepository.GetByDoctorWithAddressAsync(doctorAddressDto.AddressId, doctorId, cancellationToken);

            if (createdDoctorAddress == null)
                throw new InvalidOperationException("The created address record could not be retrieved.");

            return _mapper.Map<DoctorAddressDto>(createdDoctorAddress);
        }

        public async Task DeleteAllByDoctorAsync(Guid doctorId, CancellationToken cancellationToken)
        {
            await _doctorAddressRepository.DeleteAllByDoctorAsync(doctorId, cancellationToken);
        }

        public async Task DeleteByIdsAsync(List<Guid> addressIds, Guid doctorId, CancellationToken cancellationToken)
        {
            if (addressIds == null || addressIds.Count == 0)
                throw new ArgumentException("The list of IDs cannot be empty.", nameof(addressIds));

            var doctorAddresses = await _doctorAddressRepository.GetManyByDoctorAndAddressIdsAsync(addressIds, doctorId, cancellationToken);

            if (doctorAddresses.Count == 0)
                throw new UnauthorizedAccessException("No valid address records found for this doctor.");

            var validAddressIds = doctorAddresses.Select(da => da.AddressId).ToList();

            await _doctorAddressRepository.DeleteManyAsync(validAddressIds, doctorId, cancellationToken);
        }
    }
}
