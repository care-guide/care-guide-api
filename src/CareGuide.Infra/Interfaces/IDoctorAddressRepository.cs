using CareGuide.Models.Entities;

namespace CareGuide.Infra.Interfaces
{
    public interface IDoctorAddressRepository
    {
        Task<List<DoctorAddress>> GetAllByDoctorWithAddressesAsync(int page, int pageSize, Guid doctorId, CancellationToken cancellationToken = default);
        Task<DoctorAddress?> GetByDoctorWithAddressAsync(Guid addressId, Guid doctorId, CancellationToken cancellationToken = default);
        Task<List<DoctorAddress>> GetManyByDoctorAndAddressIdsAsync(IEnumerable<Guid> addressIds, Guid doctorId, CancellationToken cancellationToken = default);
        Task<DoctorAddress> AddAsync(DoctorAddress entity, CancellationToken cancellationToken = default);
        Task DeleteAllByDoctorAsync(Guid doctorId, CancellationToken cancellationToken = default);
        Task DeleteManyAsync(IEnumerable<Guid> addressIds, Guid doctorId, CancellationToken cancellationToken = default);
    }
}
