using AutoMapper;
using CareGuide.Core.Interfaces;
using CareGuide.Infra.Interfaces;
using CareGuide.Models.DTOs.Doctor;
using CareGuide.Models.DTOs.Shared;
using CareGuide.Models.Entities;

namespace CareGuide.Core.Services
{
    public class DoctorService : IDoctorService
    {
        private readonly IDoctorRepository _doctorRepository;
        private readonly IMapper _mapper;

        public DoctorService(IDoctorRepository doctorRepository, IMapper mapper)
        {
            _doctorRepository = doctorRepository;
            _mapper = mapper;
        }

        public async Task<PagedResult<DoctorDto>> GetAllByPersonAsync(int page, int pageSize, CancellationToken cancellationToken)
        {
            var (items, total, normalizedPage, normalizedPageSize) = await _doctorRepository.GetAllByPersonPagedAsync(page, pageSize, cancellationToken);
            return new PagedResult<DoctorDto>(_mapper.Map<List<DoctorDto>>(items), normalizedPage, normalizedPageSize, total);
        }

        public async Task<DoctorDto> GetAsync(Guid id, CancellationToken cancellationToken)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("The id cannot be empty.", nameof(id));

            var entity = await _doctorRepository.GetAsync(id, cancellationToken);

            if (entity == null)
                throw new KeyNotFoundException($"No doctor found with the ID {id}.");

            return _mapper.Map<DoctorDto>(entity);
        }

        public async Task<DoctorDto> CreateAsync(CreateDoctorDto doctor, CancellationToken cancellationToken)
        {
            if (doctor == null)
                throw new ArgumentNullException(nameof(doctor));

            var entity = _mapper.Map<Doctor>(doctor);

            await _doctorRepository.AddAsync(entity, cancellationToken);

            return _mapper.Map<DoctorDto>(entity);
        }

        public async Task<DoctorDto> UpdateAsync(Guid id, UpdateDoctorDto doctor, CancellationToken cancellationToken)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("The id cannot be empty.", nameof(id));

            if (doctor == null)
                throw new ArgumentNullException(nameof(doctor));

            var existing = await _doctorRepository.GetAsync(id, cancellationToken);

            if (existing == null)
                throw new KeyNotFoundException($"No doctor found with the ID {id}.");

            existing.Name = doctor.Name;
            existing.Details = doctor.Details;

            var updated = await _doctorRepository.UpdateAsync(existing, cancellationToken);
            return _mapper.Map<DoctorDto>(updated);
        }

        public async Task DeleteAllByPersonAsync(CancellationToken cancellationToken)
        {
            await _doctorRepository.DeleteAllByPersonAsync(cancellationToken);
        }

        public async Task DeleteByIdsAsync(List<Guid> ids, CancellationToken cancellationToken)
        {
            await _doctorRepository.DeleteManyAsync(ids, cancellationToken);
        }
    }
}
