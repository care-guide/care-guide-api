using AutoMapper;
using CareGuide.Core.Interfaces;
using CareGuide.Infra.Interfaces;
using CareGuide.Models.DTOs.DoctorSpecialty;
using CareGuide.Models.Entities;

namespace CareGuide.Core.Services
{
    public class DoctorSpecialtyService : IDoctorSpecialtyService
    {
        private readonly IDoctorSpecialtyRepository _doctorSpecialtyRepository;
        private readonly IMapper _mapper;

        public DoctorSpecialtyService(IDoctorSpecialtyRepository doctorSpecialtyRepository, IMapper mapper)
        {
            _doctorSpecialtyRepository = doctorSpecialtyRepository;
            _mapper = mapper;
        }

        public async Task<IReadOnlyCollection<DoctorSpecialtyDto>> GetAllByDoctorAsync(int page, int pageSize, Guid doctorId, CancellationToken cancellationToken)
        {
            var specialties = await _doctorSpecialtyRepository.GetAllByDoctorAsync(page, pageSize, doctorId, cancellationToken);

            if (specialties.Count == 0)
                throw new KeyNotFoundException($"No specialty records found for the doctor with ID {doctorId}.");

            return _mapper.Map<List<DoctorSpecialtyDto>>(specialties);
        }

        public async Task<DoctorSpecialtyDto> GetAsync(Guid id, Guid doctorId, CancellationToken cancellationToken)
        {
            var specialty = await _doctorSpecialtyRepository.GetByIdAsync(id, doctorId, cancellationToken);

            if (specialty == null)
                throw new UnauthorizedAccessException("You are not authorized to access this specialty record or it does not exist.");

            return _mapper.Map<DoctorSpecialtyDto>(specialty);
        }

        public async Task<DoctorSpecialtyDto> CreateAsync(Guid doctorId, CreateDoctorSpecialtyDto doctorSpecialtyDto, CancellationToken cancellationToken)
        {
            if (doctorSpecialtyDto == null)
                throw new ArgumentNullException(nameof(doctorSpecialtyDto));

            var entity = _mapper.Map<DoctorSpecialty>(doctorSpecialtyDto);
            entity.DoctorId = doctorId;

            var created = await _doctorSpecialtyRepository.AddAsync(entity, cancellationToken);

            return _mapper.Map<DoctorSpecialtyDto>(created);
        }

        public async Task<DoctorSpecialtyDto> UpdateAsync(Guid id, Guid doctorId, UpdateDoctorSpecialtyDto doctorSpecialtyDto, CancellationToken cancellationToken)
        {
            if (doctorSpecialtyDto == null)
                throw new ArgumentNullException(nameof(doctorSpecialtyDto));

            if (doctorSpecialtyDto.Id != id)
                throw new ArgumentException("The route ID must match the body ID.", nameof(id));

            var specialty = await _doctorSpecialtyRepository.GetByIdAsync(id, doctorId, cancellationToken);

            if (specialty == null)
                throw new UnauthorizedAccessException("You are not authorized to update this specialty record or it does not exist.");

            specialty.Name = doctorSpecialtyDto.Name;

            await _doctorSpecialtyRepository.UpdateAsync(specialty, cancellationToken);

            return _mapper.Map<DoctorSpecialtyDto>(specialty);
        }

        public async Task DeleteAllByDoctorAsync(Guid doctorId, CancellationToken cancellationToken)
        {
            var specialties = await _doctorSpecialtyRepository.GetAllByDoctorAsync(1, int.MaxValue, doctorId, cancellationToken);

            if (specialties.Count == 0)
                return;

            await _doctorSpecialtyRepository.DeleteAllByDoctorAsync(doctorId, cancellationToken);
        }

        public async Task DeleteByIdsAsync(List<Guid> ids, Guid doctorId, CancellationToken cancellationToken)
        {
            if (ids == null || ids.Count == 0)
                throw new ArgumentException("The list of IDs cannot be empty.", nameof(ids));

            var specialties = await _doctorSpecialtyRepository.GetManyByIdsAsync(ids, doctorId, cancellationToken);

            if (specialties.Count == 0)
                throw new UnauthorizedAccessException("No valid specialty records found for this doctor.");

            var specialtyIds = specialties.Select(x => x.Id).ToList();

            await _doctorSpecialtyRepository.DeleteManyAsync(specialtyIds, doctorId, cancellationToken);
        }
    }
}