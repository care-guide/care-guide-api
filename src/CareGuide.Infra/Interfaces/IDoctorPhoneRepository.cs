using CareGuide.Models.Entities;

namespace CareGuide.Infra.Interfaces
{
    public interface IDoctorPhoneRepository
    {
        Task AddAsync(DoctorPhone entity, CancellationToken cancellationToken = default);
        Task<List<DoctorPhone>> GetAllByDoctorWithPhonesAsync(int page, int pageSize, Guid doctorId, CancellationToken cancellationToken = default);
        Task<int> CountByDoctorAsync(Guid doctorId, CancellationToken cancellationToken = default);
        Task<List<DoctorPhone>> GetAllByDoctorUnpagedAsync(Guid doctorId, CancellationToken cancellationToken = default);
        Task<DoctorPhone?> GetByDoctorWithPhoneAsync(Guid phoneId, Guid doctorId, CancellationToken cancellationToken = default);
        Task<List<DoctorPhone>> GetManyByDoctorAndPhoneIdsAsync(IEnumerable<Guid> phoneIds, Guid doctorId, CancellationToken cancellationToken = default);
        Task DeleteAllByDoctorAsync(Guid doctorId, CancellationToken cancellationToken = default);
        Task DeleteManyAsync(IEnumerable<Guid> phoneIds, Guid doctorId, CancellationToken cancellationToken = default);
    }
}
