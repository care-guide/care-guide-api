using AutoMapper;
using CareGuide.Core.Interfaces;
using CareGuide.Infra.Interfaces;
using CareGuide.Models.DTOs.Phone;
using CareGuide.Models.Entities;

namespace CareGuide.Core.Services
{
    public class PhoneService : IPhoneService
    {
        private readonly IPhoneRepository _phoneRepository;
        private readonly IMapper _mapper;

        public PhoneService(IPhoneRepository phoneRepository, IMapper mapper)
        {
            _phoneRepository = phoneRepository;
            _mapper = mapper;
        }

        public async Task<List<PhoneDto>> GetAllAsync(int page, int pageSize, CancellationToken cancellationToken)
        {
            var list = await _phoneRepository.GetAllAsync(page, pageSize, cancellationToken);
            return _mapper.Map<List<PhoneDto>>(list);
        }

        public async Task<PhoneDto> GetAsync(Guid id, CancellationToken cancellationToken)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("The id cannot be empty.", nameof(id));

            var entity = await _phoneRepository.GetAsync(id, cancellationToken);

            if (entity == null)
                throw new KeyNotFoundException($"No phone found with the ID {id}.");

            return _mapper.Map<PhoneDto>(entity);
        }

        public async Task<PhoneDto> CreateAsync(CreatePhoneDto createPhoneDto, CancellationToken cancellationToken)
        {
            if (createPhoneDto == null)
                throw new ArgumentNullException(nameof(createPhoneDto));

            var entity = _mapper.Map<Phone>(createPhoneDto);

            await _phoneRepository.AddAsync(entity, cancellationToken);

            return _mapper.Map<PhoneDto>(entity);
        }

        public async Task<PhoneDto> UpdateAsync(Guid id, UpdatePhoneDto updatePhoneDto, CancellationToken cancellationToken)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("The id cannot be empty.", nameof(id));

            if (updatePhoneDto == null)
                throw new ArgumentNullException(nameof(updatePhoneDto));

            var existing = await _phoneRepository.GetAsync(id, cancellationToken);

            if (existing == null)
                throw new KeyNotFoundException($"No phone found with the ID {id}.");

            existing.Number = updatePhoneDto.Number;
            existing.AreaCode = updatePhoneDto.AreaCode;
            existing.Type = updatePhoneDto.Type;

            var updated = await _phoneRepository.UpdateAsync(existing, cancellationToken);
            return _mapper.Map<PhoneDto>(updated);
        }

        public async Task DeleteByIdsAsync(List<Guid> ids, CancellationToken cancellationToken)
        {
            await _phoneRepository.DeleteManyAsync(ids, cancellationToken);
        }
    }
}
