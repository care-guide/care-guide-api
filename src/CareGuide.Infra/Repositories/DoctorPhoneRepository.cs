using CareGuide.Infra.Contexts;
using CareGuide.Infra.Interfaces;
using CareGuide.Models.Constants;
using CareGuide.Models.Entities;
using CareGuide.Security.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CareGuide.Infra.Repositories
{
    public class DoctorPhoneRepository : IDoctorPhoneRepository
    {
        private readonly DatabaseContext _context;
        private readonly IUserSessionContext _userSessionContext;

        public DoctorPhoneRepository(DatabaseContext context, IUserSessionContext userSessionContext)
        {
            _context = context;
            _userSessionContext = userSessionContext;
        }

        public async Task AddAsync(DoctorPhone entity, CancellationToken cancellationToken = default)
        {
            await _context.DoctorPhones.AddAsync(entity, cancellationToken);
        }

        public async Task<List<DoctorPhone>> GetAllByDoctorWithPhonesAsync(int page, int pageSize, Guid doctorId, CancellationToken cancellationToken = default)
        {
            (page, pageSize) = PaginationNormalizer.Normalize(page, pageSize);

            var personId = _userSessionContext.PersonId;

            return await _context.DoctorPhones
                .Include(dp => dp.Phone)
                .Where(dp => dp.DoctorId == doctorId && dp.Doctor.PersonId == personId)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);
        }

        public async Task<int> CountByDoctorAsync(Guid doctorId, CancellationToken cancellationToken = default)
        {
            var personId = _userSessionContext.PersonId;

            return await _context.DoctorPhones
                .Where(dp => dp.DoctorId == doctorId && dp.Doctor.PersonId == personId)
                .CountAsync(cancellationToken);
        }

        public async Task<List<DoctorPhone>> GetAllByDoctorUnpagedAsync(Guid doctorId, CancellationToken cancellationToken = default)
        {
            var personId = _userSessionContext.PersonId;

            return await _context.DoctorPhones
                .Where(dp => dp.DoctorId == doctorId && dp.Doctor.PersonId == personId)
                .ToListAsync(cancellationToken);
        }

        public async Task<DoctorPhone?> GetByDoctorWithPhoneAsync(Guid phoneId, Guid doctorId, CancellationToken cancellationToken = default)
        {
            var personId = _userSessionContext.PersonId;

            return await _context.DoctorPhones
                .Include(dp => dp.Phone)
                .FirstOrDefaultAsync(dp => dp.DoctorId == doctorId && dp.PhoneId == phoneId && dp.Doctor.PersonId == personId, cancellationToken);
        }

        public async Task<List<DoctorPhone>> GetManyByDoctorAndPhoneIdsAsync(IEnumerable<Guid> phoneIds, Guid doctorId, CancellationToken cancellationToken = default)
        {
            var personId = _userSessionContext.PersonId;

            return await _context.DoctorPhones
                .Include(dp => dp.Phone)
                .Where(dp => dp.DoctorId == doctorId && phoneIds.Contains(dp.PhoneId) && dp.Doctor.PersonId == personId)
                .ToListAsync(cancellationToken);
        }

        public async Task DeleteAllByDoctorAsync(Guid doctorId, CancellationToken cancellationToken = default)
        {
            var personId = _userSessionContext.PersonId;

            var entities = await _context.DoctorPhones
                .Where(dp => dp.DoctorId == doctorId && dp.Doctor.PersonId == personId)
                .ToListAsync(cancellationToken);

            _context.DoctorPhones.RemoveRange(entities);
        }

        public async Task DeleteManyAsync(IEnumerable<Guid> phoneIds, Guid doctorId, CancellationToken cancellationToken = default)
        {
            var personId = _userSessionContext.PersonId;

            var entities = await _context.DoctorPhones
                .Where(dp => dp.DoctorId == doctorId && phoneIds.Contains(dp.PhoneId) && dp.Doctor.PersonId == personId)
                .ToListAsync(cancellationToken);

            _context.DoctorPhones.RemoveRange(entities);
        }
    }
}