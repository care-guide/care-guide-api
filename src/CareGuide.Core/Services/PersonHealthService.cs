using AutoMapper;
using CareGuide.Core.Interfaces;
using CareGuide.Infra.Interfaces;
using CareGuide.Models.DTOs.PersonHealth;
using CareGuide.Models.DTOs.Shared;
using CareGuide.Models.Entities;

namespace CareGuide.Core.Services
{
    public class PersonHealthService : IPersonHealthService
    {
        private readonly IPersonHealthRepository _personHealthRepository;
        private readonly IMapper _mapper;

        public PersonHealthService(IPersonHealthRepository personHealthRepository, IMapper mapper)
        {
            _personHealthRepository = personHealthRepository;
            _mapper = mapper;
        }

        public async Task<PagedResult<PersonHealthDto>> GetAllByPersonAsync(int page, int pageSize, CancellationToken cancellationToken)
        {
            var (items, total, normalizedPage, normalizedPageSize) = await _personHealthRepository.GetAllByPersonPagedAsync(page, pageSize, cancellationToken);
            return new PagedResult<PersonHealthDto>(_mapper.Map<List<PersonHealthDto>>(items), normalizedPage, normalizedPageSize, total);
        }

        public async Task<PersonHealthDto> GetAsync(Guid id, CancellationToken cancellationToken)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("The id cannot be empty.", nameof(id));

            var entity = await _personHealthRepository.GetAsync(id, cancellationToken);

            if (entity == null)
                throw new KeyNotFoundException($"No person health found with the ID {id}.");

            return _mapper.Map<PersonHealthDto>(entity);
        }

        public async Task<PersonHealthDto> CreateAsync(CreatePersonHealthDto personHealth, CancellationToken cancellationToken)
        {
            if (personHealth == null)
                throw new ArgumentNullException(nameof(personHealth));

            var entity = _mapper.Map<PersonHealth>(personHealth);

            await _personHealthRepository.AddAsync(entity, cancellationToken);

            return _mapper.Map<PersonHealthDto>(entity);
        }

        public async Task<PersonHealthDto> UpdateAsync(Guid id, UpdatePersonHealthDto personHealth, CancellationToken cancellationToken)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("The id cannot be empty.", nameof(id));

            if (personHealth == null)
                throw new ArgumentNullException(nameof(personHealth));

            var existing = await _personHealthRepository.GetAsync(id, cancellationToken);

            if (existing == null)
                throw new KeyNotFoundException($"No person health found with the ID {id}.");

            existing.BloodType = personHealth.BloodType;
            existing.Height = personHealth.Height;
            existing.Weight = personHealth.Weight;
            existing.Description = personHealth.Description;

            var updated = await _personHealthRepository.UpdateAsync(existing, cancellationToken);
            return _mapper.Map<PersonHealthDto>(updated);
        }

        public async Task DeleteAllByPersonAsync(CancellationToken cancellationToken)
        {
            await _personHealthRepository.DeleteAllByPersonAsync(cancellationToken);
        }

        public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("The id cannot be empty.", nameof(id));

            var entity = await _personHealthRepository.GetAsync(id, cancellationToken);

            if (entity == null)
                throw new KeyNotFoundException($"Person health with id {id} not found.");

            await _personHealthRepository.DeleteAsync(entity.Id, cancellationToken);
        }
    }
}