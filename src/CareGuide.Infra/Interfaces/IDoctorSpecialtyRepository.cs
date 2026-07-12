using CareGuide.Models.Entities;

namespace CareGuide.Infra.Interfaces
{
    public interface IDoctorSpecialtyRepository
    {
        Task<List<DoctorSpecialty>> GetAllByDoctorAsync(int page, int pageSize, Guid doctorId, CancellationToken cancellationToken = default);
        Task<int> CountByDoctorAsync(Guid doctorId, CancellationToken cancellationToken = default);
        Task<List<DoctorSpecialty>> GetAllByDoctorUnpagedAsync(Guid doctorId, CancellationToken cancellationToken = default);
        Task<DoctorSpecialty?> GetByIdAsync(Guid id, Guid doctorId, CancellationToken cancellationToken = default);
        Task<List<DoctorSpecialty>> GetManyByIdsAsync(IEnumerable<Guid> ids, Guid doctorId, CancellationToken cancellationToken = default);
        Task<DoctorSpecialty> AddAsync(DoctorSpecialty entity, CancellationToken cancellationToken = default);
        Task UpdateAsync(DoctorSpecialty entity, CancellationToken cancellationToken = default);
        Task DeleteAllByDoctorAsync(Guid doctorId, CancellationToken cancellationToken = default);
        Task DeleteManyAsync(IEnumerable<Guid> ids, Guid doctorId, CancellationToken cancellationToken = default);
    }
}
