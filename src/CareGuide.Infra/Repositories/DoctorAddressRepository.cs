using CareGuide.Infra.Contexts;
using CareGuide.Infra.Interfaces;
using CareGuide.Models.Constants;
using CareGuide.Models.Entities;
using CareGuide.Security.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CareGuide.Infra.Repositories
{
    public class DoctorAddressRepository : IDoctorAddressRepository
    {
        private readonly DatabaseContext _context;
        private readonly IUserSessionContext _userSessionContext;

        public DoctorAddressRepository(DatabaseContext context, IUserSessionContext userSessionContext)
        {
            _context = context;
            _userSessionContext = userSessionContext;
        }

        public async Task<List<DoctorAddress>> GetAllByDoctorWithAddressesAsync(int page, int pageSize, Guid doctorId, CancellationToken cancellationToken = default)
        {
            (page, pageSize) = PaginationNormalizer.Normalize(page, pageSize);

            var personId = _userSessionContext.PersonId;

            return await _context.DoctorAddresses
                .Include(da => da.Address)
                .Where(da => da.DoctorId == doctorId && da.Doctor.PersonId == personId)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);
        }

        public async Task<int> CountByDoctorAsync(Guid doctorId, CancellationToken cancellationToken = default)
        {
            var personId = _userSessionContext.PersonId;

            return await _context.DoctorAddresses
                .Where(da => da.DoctorId == doctorId && da.Doctor.PersonId == personId)
                .CountAsync(cancellationToken);
        }

        public async Task<DoctorAddress?> GetByDoctorWithAddressAsync(Guid addressId, Guid doctorId, CancellationToken cancellationToken = default)
        {
            var personId = _userSessionContext.PersonId;

            return await _context.DoctorAddresses
                .Include(da => da.Address)
                .FirstOrDefaultAsync(da => da.DoctorId == doctorId && da.AddressId == addressId && da.Doctor.PersonId == personId, cancellationToken);
        }

        public async Task<List<DoctorAddress>> GetManyByDoctorAndAddressIdsAsync(IEnumerable<Guid> addressIds, Guid doctorId, CancellationToken cancellationToken = default)
        {
            var personId = _userSessionContext.PersonId;

            return await _context.DoctorAddresses
                .Include(da => da.Address)
                .Where(da => da.DoctorId == doctorId && addressIds.Contains(da.AddressId) && da.Doctor.PersonId == personId)
                .ToListAsync(cancellationToken);
        }

        public async Task<DoctorAddress> AddAsync(DoctorAddress entity, CancellationToken cancellationToken = default)
        {
            await _context.DoctorAddresses.AddAsync(entity, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return entity;
        }

        public async Task DeleteAllByDoctorAsync(Guid doctorId, CancellationToken cancellationToken = default)
        {
            var personId = _userSessionContext.PersonId;

            var entities = await _context.DoctorAddresses
                .Where(da => da.DoctorId == doctorId && da.Doctor.PersonId == personId)
                .ToListAsync(cancellationToken);

            _context.DoctorAddresses.RemoveRange(entities);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteManyAsync(IEnumerable<Guid> addressIds, Guid doctorId, CancellationToken cancellationToken = default)
        {
            var personId = _userSessionContext.PersonId;

            var entities = await _context.DoctorAddresses
                .Where(da => da.DoctorId == doctorId && addressIds.Contains(da.AddressId) && da.Doctor.PersonId == personId)
                .ToListAsync(cancellationToken);

            _context.DoctorAddresses.RemoveRange(entities);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
