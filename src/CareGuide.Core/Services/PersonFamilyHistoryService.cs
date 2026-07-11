using AutoMapper;
using CareGuide.Core.Interfaces;
using CareGuide.Infra.Interfaces;
using CareGuide.Models.DTOs.PersonFamilyHistory;
using CareGuide.Models.DTOs.Shared;
using CareGuide.Models.Entities;

namespace CareGuide.Core.Services
{
    public class PersonFamilyHistoryService : IPersonFamilyHistoryService
    {
        private readonly IPersonFamilyHistoryRepository _personFamilyHistoryRepository;
        private readonly IMapper _mapper;

        public PersonFamilyHistoryService(IPersonFamilyHistoryRepository personFamilyHistoryRepository, IMapper mapper)
        {
            _personFamilyHistoryRepository = personFamilyHistoryRepository;
            _mapper = mapper;
        }

        public async Task<PagedResult<PersonFamilyHistoryDto>> GetAllByPersonAsync(int page, int pageSize, CancellationToken cancellationToken)
        {
            var (items, total, normalizedPage, normalizedPageSize) = await _personFamilyHistoryRepository.GetAllByPersonPagedAsync(page, pageSize, cancellationToken);
            return new PagedResult<PersonFamilyHistoryDto>(_mapper.Map<List<PersonFamilyHistoryDto>>(items), normalizedPage, normalizedPageSize, total);
        }

        public async Task<PersonFamilyHistoryDto> GetAsync(Guid id, CancellationToken cancellationToken)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("The id cannot be empty.", nameof(id));

            var entity = await _personFamilyHistoryRepository.GetAsync(id, cancellationToken);

            if (entity == null)
                throw new KeyNotFoundException($"No person family history found with the ID {id}.");

            return _mapper.Map<PersonFamilyHistoryDto>(entity);
        }

        public async Task<PersonFamilyHistoryDto> CreateAsync(CreatePersonFamilyHistoryDto personFamilyHistory, CancellationToken cancellationToken)
        {
            if (personFamilyHistory == null)
                throw new ArgumentNullException(nameof(personFamilyHistory));

            var entity = _mapper.Map<PersonFamilyHistory>(personFamilyHistory);

            await _personFamilyHistoryRepository.AddAsync(entity, cancellationToken);

            return _mapper.Map<PersonFamilyHistoryDto>(entity);
        }

        public async Task<PersonFamilyHistoryDto> UpdateAsync(Guid id, UpdatePersonFamilyHistoryDto personFamilyHistory, CancellationToken cancellationToken)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("The id cannot be empty.", nameof(id));

            if (personFamilyHistory == null)
                throw new ArgumentNullException(nameof(personFamilyHistory));

            var existing = await _personFamilyHistoryRepository.GetAsync(id, cancellationToken);

            if (existing == null)
                throw new KeyNotFoundException($"No person family history found with the ID {id}.");

            existing.Relationship = personFamilyHistory.Relationship;
            existing.Diagnosis = personFamilyHistory.Diagnosis;
            existing.AgeAtDiagnosis = personFamilyHistory.AgeAtDiagnosis;

            var updated = await _personFamilyHistoryRepository.UpdateAsync(existing, cancellationToken);
            return _mapper.Map<PersonFamilyHistoryDto>(updated);
        }

        public async Task DeleteAllByPersonAsync(CancellationToken cancellationToken)
        {
            await _personFamilyHistoryRepository.DeleteAllByPersonAsync(cancellationToken);
        }

        public async Task DeleteByIdsAsync(List<Guid> ids, CancellationToken cancellationToken)
        {
            await _personFamilyHistoryRepository.DeleteManyAsync(ids, cancellationToken);
        }
    }
}
