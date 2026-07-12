using AutoMapper;
using CareGuide.Core.Interfaces;
using CareGuide.Infra.Interfaces;
using CareGuide.Models.DTOs.PersonDisease;
using CareGuide.Models.DTOs.Shared;
using CareGuide.Models.Entities;

namespace CareGuide.Core.Services
{
    public class PersonDiseaseService : IPersonDiseaseService
    {
        private readonly IPersonDiseaseRepository _personDiseaseRepository;
        private readonly IMapper _mapper;

        public PersonDiseaseService(IPersonDiseaseRepository personDiseaseRepository, IMapper mapper)
        {
            _personDiseaseRepository = personDiseaseRepository;
            _mapper = mapper;
        }

        public async Task<PagedResult<PersonDiseaseDto>> GetAllByPersonAsync(int page, int pageSize, CancellationToken cancellationToken)
        {
            var (items, total, normalizedPage, normalizedPageSize) = await _personDiseaseRepository.GetAllByPersonPagedAsync(page, pageSize, cancellationToken);
            return new PagedResult<PersonDiseaseDto>(_mapper.Map<List<PersonDiseaseDto>>(items), normalizedPage, normalizedPageSize, total);
        }

        public async Task<PersonDiseaseDto> GetAsync(Guid id, CancellationToken cancellationToken)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("The id cannot be empty.", nameof(id));

            var entity = await _personDiseaseRepository.GetAsync(id, cancellationToken);

            if (entity == null)
                throw new KeyNotFoundException($"No person disease found with the ID {id}.");

            return _mapper.Map<PersonDiseaseDto>(entity);
        }

        public async Task<PersonDiseaseDto> CreateAsync(CreatePersonDiseaseDto personDisease, CancellationToken cancellationToken)
        {
            if (personDisease == null)
                throw new ArgumentNullException(nameof(personDisease));

            var entity = _mapper.Map<PersonDisease>(personDisease);

            await _personDiseaseRepository.AddAsync(entity, cancellationToken);

            return _mapper.Map<PersonDiseaseDto>(entity);
        }

        public async Task<PersonDiseaseDto> UpdateAsync(Guid id, UpdatePersonDiseaseDto personDisease, CancellationToken cancellationToken)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("The id cannot be empty.", nameof(id));

            if (personDisease == null)
                throw new ArgumentNullException(nameof(personDisease));

            var existing = await _personDiseaseRepository.GetAsync(id, cancellationToken);

            if (existing == null)
                throw new KeyNotFoundException($"No person disease found with the ID {id}.");

            existing.Name = personDisease.Name;
            existing.DiagnosisDate = personDisease.DiagnosisDate;
            existing.DiseaseType = personDisease.DiseaseType;

            var updated = await _personDiseaseRepository.UpdateAsync(existing, cancellationToken);
            return _mapper.Map<PersonDiseaseDto>(updated);
        }

        public async Task DeleteAllByPersonAsync(CancellationToken cancellationToken)
        {
            await _personDiseaseRepository.DeleteAllByPersonAsync(cancellationToken);
        }

        public async Task DeleteByIdsAsync(List<Guid> ids, CancellationToken cancellationToken)
        {
            await _personDiseaseRepository.DeleteManyAsync(ids, cancellationToken);
        }
    }
}
