using AutoMapper;
using CareGuide.Core.Interfaces;
using CareGuide.Infra.Interfaces;
using CareGuide.Models.DTOs.PersonAnnotation;
using CareGuide.Models.DTOs.Shared;
using CareGuide.Models.Entities;

namespace CareGuide.Core.Services
{
    public class PersonAnnotationService : IPersonAnnotationService
    {
        private readonly IPersonAnnotationRepository _personAnnotationRepository;
        private readonly IMapper _mapper;

        public PersonAnnotationService(IPersonAnnotationRepository personAnnotationRepository, IMapper mapper)
        {
            _personAnnotationRepository = personAnnotationRepository;
            _mapper = mapper;
        }

        public async Task<PagedResult<PersonAnnotationDto>> GetAllByPersonAsync(int page, int pageSize, CancellationToken cancellationToken)
        {
            var (items, total, normalizedPage, normalizedPageSize) = await _personAnnotationRepository.GetAllByPersonPagedAsync(page, pageSize, cancellationToken);
            return new PagedResult<PersonAnnotationDto>(_mapper.Map<List<PersonAnnotationDto>>(items), normalizedPage, normalizedPageSize, total);
        }

        public async Task<PersonAnnotationDto> GetAsync(Guid id, CancellationToken cancellationToken)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("The id cannot be empty.", nameof(id));

            var entity = await _personAnnotationRepository.GetAsync(id, cancellationToken);

            if (entity == null)
                throw new KeyNotFoundException($"No person annotation found with the ID {id}.");

            return _mapper.Map<PersonAnnotationDto>(entity);
        }

        public async Task<PersonAnnotationDto> CreateAsync(CreatePersonAnnotationDto personAnnotation, CancellationToken cancellationToken)
        {
            if (personAnnotation == null)
                throw new ArgumentNullException(nameof(personAnnotation));

            var entity = _mapper.Map<PersonAnnotation>(personAnnotation);

            await _personAnnotationRepository.AddAsync(entity, cancellationToken);

            return _mapper.Map<PersonAnnotationDto>(entity);
        }

        public async Task<PersonAnnotationDto> UpdateAsync(Guid id, UpdatePersonAnnotationDto personAnnotation, CancellationToken cancellationToken)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("The id cannot be empty.", nameof(id));

            if (personAnnotation == null)
                throw new ArgumentNullException(nameof(personAnnotation));

            var existing = await _personAnnotationRepository.GetAsync(id, cancellationToken);

            if (existing == null)
                throw new KeyNotFoundException($"No person annotation found with the ID {id}.");

            existing.Details = personAnnotation.Details;
            existing.FileUrl = personAnnotation.FileUrl;

            var updated = await _personAnnotationRepository.UpdateAsync(existing, cancellationToken);
            return _mapper.Map<PersonAnnotationDto>(updated);
        }

        public async Task DeleteAllByPersonAsync(CancellationToken cancellationToken)
        {
            await _personAnnotationRepository.DeleteAllByPersonAsync(cancellationToken);
        }

        public async Task DeleteByIdsAsync(List<Guid> ids, CancellationToken cancellationToken)
        {
            await _personAnnotationRepository.DeleteManyAsync(ids, cancellationToken);
        }
    }
}
